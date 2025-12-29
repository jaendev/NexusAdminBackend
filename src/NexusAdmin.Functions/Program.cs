using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexusAdmin.Infrastructure.Configuration;
using NexusAdmin.Core.UseCases.Users.CreateUser;
using NexusAdmin.Functions.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        // services.ConfigureFunctionsApplicationInsights();
        
        // Infrastructure (Repositories + Services)
        services.AddInfrastructure(context.Configuration);
        
        // Global JSON configuration
        services.Configure<JsonSerializerOptions>(options =>
        {
            options.PropertyNameCaseInsensitive = true;
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.WriteIndented = true;
        });
        
        // Use Cases
        services.AddScoped<CreateUserUseCase>();
    })
    .Build();

host.Run();