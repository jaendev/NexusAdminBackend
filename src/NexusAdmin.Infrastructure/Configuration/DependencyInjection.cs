using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NexusAdmin.Core.Interfaces.Repositories;
using NexusAdmin.Core.Interfaces.Services;
using NexusAdmin.Infrastructure.Persistence.MongoDB.Configuration;
using NexusAdmin.Infrastructure.Persistence.MongoDB.Repositories;
using NexusAdmin.Infrastructure.Services.Email;

namespace NexusAdmin.Infrastructure.Configuration;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // MongoDB Configuration
        services.Configure<MongoDbSettings>(
            configuration.GetSection("MongoDB")
        );

        services.AddSingleton<MongoDbContext>();

        // Repositories (con logging)
        services.AddScoped<IUserRepository, MongoDbUserRepository>();

        // Email Configuration
        services.Configure<EmailSettings>(
            configuration.GetSection("Email")
        );

        // Services
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}