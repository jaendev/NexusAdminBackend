using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// using NexusAdmin.Infrastructure.Configuration;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        // services.ConfigureFunctionsApplicationInsights();

        // Registrar Infrastructure (Repositories + Services)
        // services.AddInfrastructure(context.Configuration);

        // Register Use Cases
        // TODO: Add user when create
        // services.AddScoped<CreateUserUseCase>();
    })
    .Build();

host.Run();