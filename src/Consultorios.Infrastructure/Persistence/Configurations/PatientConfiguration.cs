using Consultorios.Domain.Patients;
using Consultorios.Domain.Tenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Consultorios.Infrastructure.Persistence.Configurations;

public sealed class PatientConfiguration : IEntityTypeConfiguration<Patient>
{
    public void Configure(EntityTypeBuilder<Patient> builder)
    {
        builder.ToTable("Patients");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.LastName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.FirstName).HasMaxLength(120).IsRequired();
        builder.Property(x => x.NationalId).HasMaxLength(32).IsRequired();
        builder.Property(x => x.Phone).HasMaxLength(40);
        builder.HasIndex(x => new { x.TenantId, x.NationalId }).IsUnique();
        builder.HasIndex(x => new { x.TenantId, x.LastName, x.FirstName });

        builder.OwnsOne(x => x.Address, address =>
        {
            address.Property(p => p.Street).HasMaxLength(200);
            address.Property(p => p.Number).HasMaxLength(20);
            address.Property(p => p.Apartment).HasMaxLength(20);
            address.Property(p => p.City).HasMaxLength(120);
            address.Property(p => p.Province).HasMaxLength(120);
            address.Property(p => p.PostalCode).HasMaxLength(20);
        });

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(x => x.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
