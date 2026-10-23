using Consultorios.Domain.Common;
using Consultorios.Domain.HealthPlans;
using Consultorios.Domain.Tests.Support;
using FluentAssertions;

namespace Consultorios.Domain.Tests.HealthPlans;

public class PatientCoverageTests
{
    [Fact]
    public void Coverage_stores_plan_name_and_affiliation_number()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var plan = HealthPlan.Create(tenant.Id, "OS Demo");

        var coverage = PatientCoverage.Create(tenant.Id, patient.Id, plan.Id, "AFF-001");

        coverage.AffiliationNumber.Should().Be("AFF-001");
        coverage.HealthPlanId.Should().Be(plan.Id);
        plan.Name.Should().Be("OS Demo");
    }

    [Fact]
    public void Coverage_requires_affiliation_number()
    {
        var tenant = DomainFixtures.ClinicA();
        var patient = DomainFixtures.DemoPatient(tenant.Id);
        var plan = HealthPlan.Create(tenant.Id, "OS Demo");

        var act = () => PatientCoverage.Create(tenant.Id, patient.Id, plan.Id, " ");
        act.Should().Throw<DomainException>().WithMessage("Affiliation number is required.");
    }
}
