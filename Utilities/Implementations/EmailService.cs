using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Utilities.Interfaces;

namespace Utilities.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string message)
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"]);
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPass = _configuration["Email:SmtpPass"];
            var fromEmail = _configuration["Email:From"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var subject = "Notificación del Sistema";

            var htmlBody = $@"
                <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px; }}
                            .container {{ background-color: #fff; padding: 20px; border-radius: 8px; box-shadow: 0 0 10px rgba(0,0,0,0.1); }}
                            .footer {{ margin-top: 20px; font-size: 12px; color: #888; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <h2>Hola,</h2>
                            <p>{message}</p>
                            <p>Gracias por usar nuestro sistema.</p>
                            <div class='footer'>Este correo fue generado automáticamente, por favor no responda.</div>
                        </div>
                    </body>
                </html>";

            var mailMessage = new MailMessage(fromEmail, to, subject, htmlBody)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}
