using FluentAssertions;

namespace Consultorios.Application.Tests;

public class SolutionSmokeTests
{
    [Fact]
    public void Application_assembly_is_loadable()
    {
        typeof(DependencyInjection).Assembly.GetName().Name.Should().Be("Consultorios.Application");
    }
}
