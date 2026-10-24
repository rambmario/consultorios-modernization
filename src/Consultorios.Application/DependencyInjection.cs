using Consultorios.Application.Catalog;
using Consultorios.Application.Clinical;
using Consultorios.Application.Patients;
using Consultorios.Application.Scheduling;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Consultorios.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        services.AddScoped<PatientService>();
        services.AddScoped<AppointmentService>();
        services.AddScoped<CatalogService>();
        services.AddScoped<ClinicalService>();
        return services;
    }
}
