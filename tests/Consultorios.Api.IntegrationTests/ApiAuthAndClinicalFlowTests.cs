using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Consultorios.Api.IntegrationTests.Infrastructure;
using Consultorios.Application.Auth;
using Consultorios.Application.Clinical;
using Consultorios.Application.Common;
using Consultorios.Application.Patients;
using Consultorios.Application.Scheduling;
using Consultorios.Domain.Clinical;
using Consultorios.Infrastructure.Persistence.Seed;
using FluentAssertions;

namespace Consultorios.Api.IntegrationTests;

[Collection(PostgresCollection.Name)]
public sealed class ApiAuthAndClinicalFlowTests : IAsyncLifetime
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly PostgresFixture _postgres;
    private ConsultoriosApiFactory? _factory;
    private HttpClient? _client;
    private HttpClient Client => _client ?? throw new InvalidOperationException("HTTP client was not initialized.");

    public ApiAuthAndClinicalFlowTests(PostgresFixture postgres)
    {
        _postgres = postgres;
    }

    public async Task InitializeAsync()
    {
        await ApiDatabase.EnsureCreatedAsync(_postgres.ConnectionString);
        _factory = new ConsultoriosApiFactory(_postgres);
        _client = _factory.CreateClient();
        await _client.GetAsync("/health");
    }

    public async Task DisposeAsync()
    {
        _client?.Dispose();
        if (_factory is not null)
        {
            await _factory.DisposeAsync();
        }
    }

    [Fact]
    public async Task Login_returns_jwt_with_tenant_and_role()
    {
        var login = await LoginAsync(DemoIds.ProfesionalClinicA, TestAuth.ProfesionalPassword);
        login.AccessToken.Should().NotBeNullOrWhiteSpace();
        login.TenantId.Should().NotBe(Guid.Empty);
        login.Roles.Should().Contain(AppRoles.Profesional);
    }

    [Fact]
    public async Task Reception_can_schedule_but_cannot_read_clinical_record()
    {
        var reception = await LoginAsync(DemoIds.RecepcionClinicA, TestAuth.ReceptionPassword);
        Authorize(reception.AccessToken);

        var patients = await Client.GetFromJsonAsync<PagedResult<PatientResponse>>("/api/patients", JsonOptions);
        var patient = patients!.Items.Should().ContainSingle(p => p.FirstName == "Paciente Demo 1").Subject;

        var start = new DateTimeOffset(2026, 11, 2, 15, 0, 0, TimeSpan.Zero);
        var created = await Client.PostAsJsonAsync("/api/appointments", new CreateAppointmentRequest(
            patient.Id,
            DemoIds.ProfesionalA,
            start,
            start.AddMinutes(30)));
        created.StatusCode.Should().Be(HttpStatusCode.Created);

        var clinical = await Client.GetAsync($"/api/patients/{patient.Id}/clinical-record");
        clinical.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Professional_reads_clinical_record_and_writes_audit()
    {
        var login = await LoginAsync(DemoIds.ProfesionalClinicA, TestAuth.ProfesionalPassword);
        Authorize(login.AccessToken);

        var patients = await Client.GetFromJsonAsync<PagedResult<PatientResponse>>("/api/patients", JsonOptions);
        var patient = patients!.Items.Should().ContainSingle(p => p.FirstName == "Paciente Demo 1").Subject;

        var record = await Client.GetFromJsonAsync<ClinicalRecordResponse>(
            $"/api/patients/{patient.Id}/clinical-record",
            JsonOptions);
        record.Should().NotBeNull();
        record!.ProgressNotes.Should().NotBeEmpty();

        var audits = await Client.GetFromJsonAsync<List<ClinicalAuditResponse>>(
            $"/api/clinical-audits?patientId={patient.Id}",
            JsonOptions);
        audits.Should().Contain(a =>
            a.Action == ClinicalAccessAction.Viewed
            && a.ResourceType == "ClinicalRecord"
            && a.ActorUserId == DemoIds.ProfesionalA);
    }

    [Fact]
    public async Task Other_professional_view_is_audited_but_cannot_cancel_foreign_note()
    {
        var owner = await LoginAsync(DemoIds.ProfesionalClinicA, TestAuth.ProfesionalPassword);
        Authorize(owner.AccessToken);
        var patients = await Client.GetFromJsonAsync<PagedResult<PatientResponse>>("/api/patients", JsonOptions);
        var patient = patients!.Items.Single(p => p.FirstName == "Paciente Demo 1");
        var notes = await Client.GetFromJsonAsync<List<ProgressNoteResponse>>(
            $"/api/patients/{patient.Id}/progress-notes",
            JsonOptions);
        var noteId = notes!.First().Id;

        var other = await LoginAsync(DemoIds.Profesional2ClinicA, TestAuth.ProfesionalPassword);
        Authorize(other.AccessToken);

        var view = await Client.GetAsync($"/api/patients/{patient.Id}/clinical-record");
        view.StatusCode.Should().Be(HttpStatusCode.OK);

        var cancel = await Client.PostAsJsonAsync(
            $"/api/progress-notes/{noteId}/cancel",
            new CancelProgressNoteRequest("No es mia"));
        cancel.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);

        var audits = await Client.GetFromJsonAsync<List<ClinicalAuditResponse>>(
            $"/api/clinical-audits?patientId={patient.Id}",
            JsonOptions);
        audits.Should().Contain(a =>
            a.Action == ClinicalAccessAction.Viewed && a.ActorUserId == DemoIds.ProfesionalA2);
    }

    [Fact]
    public async Task Tenant_A_requesting_tenant_B_patient_returns_404()
    {
        var clinicB = await LoginAsync(DemoIds.RecepcionClinicB, TestAuth.ReceptionPassword);
        Authorize(clinicB.AccessToken);
        var patientsB = await Client.GetFromJsonAsync<PagedResult<PatientResponse>>("/api/patients", JsonOptions);
        var foreignId = patientsB!.Items.Should().ContainSingle(p => p.FirstName == "Paciente Demo 2").Subject.Id;

        var clinicA = await LoginAsync(DemoIds.RecepcionClinicA, TestAuth.ReceptionPassword);
        Authorize(clinicA.AccessToken);
        var response = await Client.GetAsync($"/api/patients/{foreignId}");
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Admin_cannot_read_clinical_record()
    {
        var admin = await LoginAsync(DemoIds.AdminClinicA, TestAuth.AdminPassword);
        Authorize(admin.AccessToken);

        var patients = await Client.GetFromJsonAsync<PagedResult<PatientResponse>>("/api/patients", JsonOptions);
        var patient = patients!.Items.Single(p => p.FirstName == "Paciente Demo 1");

        var clinical = await Client.GetAsync($"/api/patients/{patient.Id}/clinical-record");
        clinical.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<LoginResponse> LoginAsync(string email, string password)
    {
        var response = await Client.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>(JsonOptions);
        body.Should().NotBeNull();
        return body!;
    }

    private void Authorize(string token)
    {
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }
}
