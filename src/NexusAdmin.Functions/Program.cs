using System.Text.Json;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NexusAdmin.Core.UseCases.Users.ActivateUser;
using NexusAdmin.Infrastructure.Configuration;
using NexusAdmin.Core.UseCases.Users.CreateUser;
using NexusAdmin.Core.UseCases.Users.DeactivateUser;
using NexusAdmin.Core.UseCases.Users.DeleteUser;
using NexusAdmin.Core.UseCases.Users.GetUser;
using NexusAdmin.Core.UseCases.Users.ListUsers;
using NexusAdmin.Core.UseCases.Users.UpdateUser;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        // Application Insights
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();
        
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
        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetUserByIdUseCase>();
        services.AddScoped<ListUsersUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<DeleteUserUseCase>();
        services.AddScoped<ActivateUserUseCase>();
        services.AddScoped<DeactivateUserUseCase>();
    })
    .Build();

host.Run();