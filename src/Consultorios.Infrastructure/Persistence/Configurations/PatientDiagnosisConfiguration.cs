using Consultorios.Domain.Clinical;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consultorios.Infrastructure.Persistence.Configurations;

public sealed class PatientDiagnosisConfiguration : IEntityTypeConfiguration<PatientDiagnosis>
{
    public void Configure(EntityTypeBuilder<PatientDiagnosis> builder)
    {
        builder.ToTable("PatientDiagnoses");
        builder.HasKey(x => x.Id);
        builder.Ignore(x => x.IsCurrent);
        builder.HasIndex(x => new { x.TenantId, x.PatientId, x.DiagnosisId });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Diagnosis>()
            .WithMany()
            .HasForeignKey(x => x.DiagnosisId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
