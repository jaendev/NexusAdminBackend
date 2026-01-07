using System.Threading.Tasks;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using NexusAdmin.Core.Interfaces.Services;

namespace NexusAdmin.Infrastructure.Services.Email;

public class SmtpEmailService : IEmailService
{
    private readonly EmailSettings _settings;

    public SmtpEmailService(IOptions<EmailSettings> settings)
    {
        this._settings = settings.Value;
    }

    public async Task SendWelcomeEmailAsync(string toEmail, string? userName)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(this._settings.FromName, this._settings.FromEmail));
        message.To.Add(new MailboxAddress(userName ?? "User", toEmail));
        message.Subject = $"Welcome to NexusAdmin, {userName ?? "User"}!";
        
        BodyBuilder bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h1 style='color: #333;'>Hello {userName ?? "User"}!</h1>
                    <p>Welcome to <strong>NexusAdmin</strong>.</p>
                    <p>Your account has been created successfully.</p>
                    <br>
                    <p style='color: #666; font-size: 12px;'>
                        This is an automated email. Please do not reply.
                    </p>
                </body>
                </html>
            ",
            TextBody = $"Hello {userName}!\n\nWelcome to NexusAdmin.\nYour account has been created successfully."
        };
        
        message.Body = bodyBuilder.ToMessageBody();
        
        using SmtpClient client = new SmtpClient();
        await client.ConnectAsync(this._settings.SmtpHost, this._settings.SmtpPort, this._settings.UseSsl);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }

    public async Task SendPasswordResetEmailAsync(string toEmail, string resetToken)
    {
        MimeMessage message = new MimeMessage();
        message.From.Add(new MailboxAddress(this._settings.FromName, this._settings.FromEmail));
        message.To.Add(new MailboxAddress("", toEmail));
        message.Subject = "Reset your NexusAdmin password";
        
        BodyBuilder bodyBuilder = new BodyBuilder
        {
            HtmlBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h1 style='color: #333;'>Password Recovery</h1>
                    <p>You have requested to reset your password.</p>
                    <p>Your recovery code is: <strong>{resetToken}</strong></p>
                    <p>This code expires in 15 minutes.</p>
                </body>
                </html>
            "
        };
        
        message.Body = bodyBuilder.ToMessageBody();
        
        using SmtpClient client = new SmtpClient();
        await client.ConnectAsync(this._settings.SmtpHost, this._settings.SmtpPort, this._settings.UseSsl);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}
