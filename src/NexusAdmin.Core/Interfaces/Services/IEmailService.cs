using System.Threading.Tasks;

namespace NexusAdmin.Core.Interfaces.Services;

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string toEmail, string? userName);
    Task SendPasswordResetEmailAsync(string toEmail, string resetToken);
}