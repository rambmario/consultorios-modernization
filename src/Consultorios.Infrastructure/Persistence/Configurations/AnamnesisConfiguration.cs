using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consultorios.Infrastructure.Persistence.Configurations;

public sealed class AnamnesisConfiguration : IEntityTypeConfiguration<Anamnesis>
{
    public void Configure(EntityTypeBuilder<Anamnesis> builder)
    {
        builder.ToTable("Anamneses");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ChiefComplaint).HasColumnType("text").IsRequired();
        builder.Property(x => x.CurrentIllnessHistory).HasColumnType("text").IsRequired();
        builder.Property(x => x.PersonalPathologicalHistory).HasColumnType("text").IsRequired();
        builder.Property(x => x.FamilyHistory).HasColumnType("text").IsRequired();
        builder.Property(x => x.PhysicalExamination).HasColumnType("text").IsRequired();
        builder.Property(x => x.HabitHistory).HasColumnType("text").IsRequired();
        builder.HasIndex(x => new { x.TenantId, x.PatientId }).IsUnique();

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
