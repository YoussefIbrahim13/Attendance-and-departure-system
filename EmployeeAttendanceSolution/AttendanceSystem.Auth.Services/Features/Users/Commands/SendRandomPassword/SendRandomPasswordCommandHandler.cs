using MailKit.Net.Smtp;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using NETCore.MailKit.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendanceSystem.Auth.Services.Features.Users.Commands.SendRandomPassword
{
    public class SendRandomPasswordCommandHandler : IRequestHandler<SendRandomPasswordCommand, SendRandomPasswordResponse>
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<SendRandomPasswordCommandHandler> _logger;

        public SendRandomPasswordCommandHandler(
            IOptions<EmailSettings> emailSettings,
            ILogger<SendRandomPasswordCommandHandler> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        public async Task<SendRandomPasswordResponse> Handle(SendRandomPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var randomPassword = GenerateRandomPassword();

                // Send email using MailKit directly
                await SendEmailAsync(request.To, randomPassword, cancellationToken);

                _logger.LogInformation("Password email sent successfully to {Email}", request.To);

                return new SendRandomPasswordResponse
                {
                    Password = randomPassword,
                    EmailSent = true
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", request.To);

                // Still return password but indicate email failed
                return new SendRandomPasswordResponse
                {
                    Password = GenerateRandomPassword(),
                    EmailSent = false,
                    Error = "Email sending failed but user can be created"
                };
            }
        }

        private async Task SendEmailAsync(string toEmail, string password, CancellationToken cancellationToken)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
            message.To.Add(new MailboxAddress("", toEmail));
            message.Subject = "Your New Account Password";

            message.Body = new TextPart("plain")
            {
                Text = $"Your new account has been created.\n\n" +
                       $"Email: {toEmail}\n" +
                       $"Temporary Password: {password}\n\n" +
                       $"Please change your password after first login."
            };

            using var client = new SmtpClient();

            // Connect to SMTP server
            await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort,
                                    MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);

            // Authenticate with credentials
            await client.AuthenticateAsync(_emailSettings.SmtpUser, _emailSettings.SmtpPass, cancellationToken);

            // Send the email
            await client.SendAsync(message, cancellationToken);

            // Disconnect
            await client.DisconnectAsync(true, cancellationToken);
        }

        private string GenerateRandomPassword()
        {
            var random = new Random();

            // Required sets
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string digits = "1234567890";
            const string special = "!@#$%^&*";

            // Pick one from each required set
            var passwordChars = new List<char>
    {
        lower[random.Next(lower.Length)],
        upper[random.Next(upper.Length)],
        digits[random.Next(digits.Length)],
        special[random.Next(special.Length)]
    };

            // Fill the rest randomly from all sets
            string allChars = lower + upper + digits + special;
            while (passwordChars.Count < 12)
            {
                passwordChars.Add(allChars[random.Next(allChars.Length)]);
            }

            // Shuffle to avoid predictable pattern
            return new string(passwordChars.OrderBy(_ => random.Next()).ToArray());
        }

    }
}
