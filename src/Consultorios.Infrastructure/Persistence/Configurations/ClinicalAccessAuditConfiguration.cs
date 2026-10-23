using Consultorios.Domain.Clinical;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consultorios.Infrastructure.Persistence.Configurations;

public sealed class ClinicalAccessAuditConfiguration : IEntityTypeConfiguration<ClinicalAccessAudit>
{
    public void Configure(EntityTypeBuilder<ClinicalAccessAudit> builder)
    {
        builder.ToTable("ClinicalAccessAudits");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ResourceType).HasMaxLength(64).IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.PatientId, x.OccurredAt });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
