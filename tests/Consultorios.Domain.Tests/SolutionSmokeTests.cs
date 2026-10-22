using Consultorios.Domain.Common;
using FluentAssertions;

namespace Consultorios.Domain.Tests;

public class SolutionSmokeTests
{
    [Fact]
    public void Domain_assembly_is_loadable()
    {
        typeof(Entity).Assembly.GetName().Name.Should().Be("Consultorios.Domain");
    }

    [Fact]
    public void New_entity_uses_guid_version_7()
    {
        var entity = new SmokeEntity();
        entity.Id.Version.Should().Be(7);
    }

    private sealed class SmokeEntity : Entity;
}
