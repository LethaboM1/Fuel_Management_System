using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FMS.Services
{
    public interface IEmailService
    {
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken);
    }

    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetToken)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                var mode = emailSettings["Mode"] ?? "LogOnly";
                var baseUrl = _configuration["App:BaseUrl"] ?? "http://localhost:3000";
                var resetLink = $"{baseUrl}/reset-password?token={resetToken}";
                
                switch (mode.ToLower())
                {
                    case "log":
                    case "logonly":
                        await LogEmail(toEmail, userName, resetToken, resetLink);
                        break;
                        
                    case "filesave":
                        await SaveEmailToFile(toEmail, userName, resetToken, resetLink);
                        break;
                        
                    case "smtpserver":
                        await SendRealEmail(toEmail, userName, resetToken, resetLink);
                        break;
                        
                    default:
                        _logger.LogWarning($"Unknown email mode: {mode}. Defaulting to LogOnly.");
                        await LogEmail(toEmail, userName, resetToken, resetLink);
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendPasswordResetEmailAsync");
                // Don't throw - we don't want password reset to fail because of email
            }
        }

        private async Task LogEmail(string toEmail, string userName, string resetToken, string resetLink)
        {
            _logger.LogInformation("=== PASSWORD RESET EMAIL (Logged) ===");
            _logger.LogInformation($"To: {toEmail}");
            _logger.LogInformation($"User: {userName}");
            _logger.LogInformation($"Reset Link: {resetLink}");
            _logger.LogInformation($"Reset Token: {resetToken}");
            _logger.LogInformation("======================================");
            await Task.CompletedTask;
        }

        private async Task SaveEmailToFile(string toEmail, string userName, string resetToken, string resetLink)
        {
            try
            {
                var emailLogPath = Path.Combine(Directory.GetCurrentDirectory(), "EmailLogs");
                Directory.CreateDirectory(emailLogPath);
                
                var fileName = $"ResetEmail_{DateTime.Now:yyyyMMdd_HHmmss}_{userName.Replace(" ", "_")}.html";
                var filePath = Path.Combine(emailLogPath, fileName);
                
                var emailBody = GenerateResetEmailBody(userName, resetLink, resetToken);
                await File.WriteAllTextAsync(filePath, emailBody);
                
                // Also save a text version
                var textContent = $"To: {toEmail}\n" +
                                $"User: {userName}\n" +
                                $"Reset Link: {resetLink}\n" +
                                $"Reset Token: {resetToken}\n" +
                                $"Time: {DateTime.Now}\n" +
                                $"Reset Link: {resetLink}";
                
                await File.WriteAllTextAsync(filePath.Replace(".html", ".txt"), textContent);
                
                _logger.LogInformation($"Email saved to: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving email to file");
            }
        }

        private async Task SendRealEmail(string toEmail, string userName, string resetToken, string resetLink)
        {
            try
            {
                var emailSettings = _configuration.GetSection("EmailSettings");
                
                var smtpServer = emailSettings["SmtpServer"];
                var smtpPort = emailSettings["SmtpPort"];
                var username = emailSettings["Username"];
                var password = emailSettings["Password"];

                if (string.IsNullOrEmpty(smtpServer))
                {
                    _logger.LogWarning("SMTP server not configured. Falling back to LogOnly.");
                    await LogEmail(toEmail, userName, resetToken, resetLink);
                    return;
                }

                if (!int.TryParse(smtpPort, out int port))
                {
                    port = 587; // Default SMTP port
                }

                using var smtpClient = new SmtpClient(smtpServer)
                {
                    Port = port,
                    EnableSsl = true,
                };

                // Only add credentials if provided
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    smtpClient.Credentials = new NetworkCredential(username, password);
                }

                var fromEmail = username ?? "noreply@fms.com";
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, "FMS System"),
                    Subject = "Password Reset Request - Fuel Management System",
                    Body = GenerateResetEmailBody(userName, resetLink, resetToken),
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to: {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending real email");
                // Fall back to logging
                await LogEmail(toEmail, userName, resetToken, resetLink);
            }
        }

        private string GenerateResetEmailBody(string userName, string resetLink, string resetToken)
        {
            return $@"
            <!DOCTYPE html>
            <html>
            <head>
                <style>
                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                    .header {{ background-color: #28a745; color: white; padding: 20px; text-align: center; }}
                    .content {{ background: #f9f9f9; padding: 20px; }}
                    .button {{ background-color: #28a745; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block; }}
                    .token {{ background: #f0f0f0; padding: 10px; border-radius: 5px; font-family: monospace; margin: 10px 0; }}
                    .note {{ background: #fff3cd; border: 1px solid #ffeaa7; padding: 10px; border-radius: 5px; margin: 10px 0; }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>
                        <h1>Fuel Management System</h1>
                    </div>
                    <div class='content'>
                        <h2>Password Reset Request</h2>
                        <p>Hi {userName},</p>
                        <p>You requested to reset your password for the Fuel Management System.</p>
                        
                        <p><strong>Click the button below to reset your password:</strong></p>
                        <p>
                            <a href='{resetLink}' class='button'>Reset Password</a>
                        </p>
                        
                        <p>Or copy and paste this token in the reset password page:</p>
                        <div class='token'>{resetToken}</div>
                        
                        <p><strong>This link will expire in 1 hour.</strong></p>
                        
                        <p>If you didn't request this reset, please ignore this email.</p>
                        
                        <p>Best regards,<br>FMS Team</p>
                    </div>
                </div>
            </body>
            </html>";
        }
    }
}