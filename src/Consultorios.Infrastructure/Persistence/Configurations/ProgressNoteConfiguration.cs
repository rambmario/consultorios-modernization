using Consultorios.Domain.Clinical;
using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consultorios.Infrastructure.Persistence.Configurations;

public sealed class ProgressNoteConfiguration : IEntityTypeConfiguration<ProgressNote>
{
    public void Configure(EntityTypeBuilder<ProgressNote> builder)
    {
        builder.ToTable("ProgressNotes");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Body).HasColumnType("text").IsRequired();
        builder.Property(x => x.CancellationReason).HasMaxLength(500);
        builder.HasIndex(x => new { x.TenantId, x.PatientId, x.SignedAt });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Patient>()
            .WithMany()
            .HasForeignKey(x => x.PatientId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<ProgressNote>()
            .WithMany()
            .HasForeignKey(x => x.CorrectsNoteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
