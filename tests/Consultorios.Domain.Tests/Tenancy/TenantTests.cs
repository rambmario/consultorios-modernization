using Consultorios.Domain.Common;
using Consultorios.Domain.Tenancy;
using FluentAssertions;

namespace Consultorios.Domain.Tests.Tenancy;

public class TenantTests
{
    [Fact]
    public void Create_trims_name_and_assigns_guid_v7()
    {
        var tenant = Tenant.Create("  ClinicaA  ");

        tenant.Name.Should().Be("ClinicaA");
        tenant.Id.Version.Should().Be(7);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_requires_name(string? name)
    {
        var act = () => Tenant.Create(name!);
        act.Should().Throw<DomainException>().WithMessage("Tenant name is required.");
    }
}
