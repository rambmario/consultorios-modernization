using Consultorios.Application;
using Consultorios.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure();
builder.Services.AddHealthChecks();

var app = builder.Build();

if (!app.Configuration.GetValue("DisableHttpsRedirection", false))
{
    app.UseHttpsRedirection();
}

app.MapHealthChecks("/health");

app.Run();

public partial class Program;