using Consultorios.Domain.HealthPlans;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consultorios.Infrastructure.Persistence.Configurations;

public sealed class PatientCoverageConfiguration : IEntityTypeConfiguration<PatientCoverage>
{
    public void Configure(EntityTypeBuilder<PatientCoverage> builder)
    {
        builder.ToTable("PatientCoverages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.AffiliationNumber).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.PatientId }).IsUnique();

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<HealthPlan>()
            .WithMany()
            .HasForeignKey(x => x.HealthPlanId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
