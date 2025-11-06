using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Infrastructure.Configuration;

namespace UniversityManagement.Infrastructure.Adapters.Out
{
    public class EmailNotificationAdapter : IEmailNotificationPort
    {
        private readonly ILogger<EmailNotificationAdapter> _logger;
        private readonly SmtpSettings _smtpSettings;

        public EmailNotificationAdapter(
            ILogger<EmailNotificationAdapter> logger, 
            IOptions<SmtpSettings> smtpSettings)
        {
            _logger = logger;
            _smtpSettings = smtpSettings.Value;
        }

        public async Task SendEnrollmentCancellation(string email, string ownerName, string course, string enrollmentDate)
        {
            try
            {                
                _logger.LogInformation("Enviando Email a {email}: {Message}", email, ownerName);

                using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };

                var message = new MailMessage(_smtpSettings.FromEmail, email)
                {
                    Subject = "Cancelación de Matrícula - Universidad",
                    Body = $@"
                        <h2>Hola {ownerName},</h2>
                        <p>Te confirmamos que tu cancelación de matrícula para <strong>{course}</strong> se completó el día {enrollmentDate}.</p>
                        <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                        <br>
                        <p>Saludos,<br>{_smtpSettings.FromName}</p>
                    ",
                    IsBodyHtml = true
                };

                message.From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
                await smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email de cancelación enviado exitosamente a {email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando Email a {email}", email);
            }
        }

        public async Task SendEnrollmentConfirmation(string email, string ownerName, string course, string enrollmentDate)
        {
            try
            {
                _logger.LogInformation("Enviando Email a {email}: {Message}", email, ownerName);

                using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };

                var message = new MailMessage(_smtpSettings.FromEmail, email)
                {
                    Subject = "Confirmación de Matrícula - Universidad",
                    Body = $@"
                        <h2>¡Hola {ownerName}!</h2>
                        <p>Te confirmamos que tu matrícula para <strong>{course}</strong> se completó exitosamente el día {enrollmentDate}.</p>
                        <p>¡Que tengas mucho éxito en tus estudios!</p>
                        <br>
                        <p>Saludos,<br>{_smtpSettings.FromName}</p>
                    ",
                    IsBodyHtml = true
                };

                message.From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
                await smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email enviado exitosamente a {email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando Email a {email}", email);
            }
        }

        public async Task SendStudentUpdateNotificationAsync(string email, string nombre, List<string> eventos)
        {
            try
            {
                _logger.LogInformation("Enviando Email de actualización a {email}: {nombre}", email, nombre);

                using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };

                var eventosTexto = string.Join("<li>", eventos.Select(e => $"<li>{e}</li>"));
                
                var message = new MailMessage(_smtpSettings.FromEmail, email)
                {
                    Subject = "Actualización - Gestión Universitaria",
                    Body = $@"
                        <h2>Hola {nombre},</h2>
                        <p>Te notificamos que han ocurrido los siguientes eventos en tu cuenta:</p>
                        <ul>
                            {eventosTexto}
                        </ul>
                        <p>Si tienes alguna pregunta, no dudes en contactarnos.</p>
                        <br>
                        <p>Saludos,<br>{_smtpSettings.FromName}</p>
                    ",
                    IsBodyHtml = true
                };

                message.From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
                await smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email de actualización enviado exitosamente a {email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando Email a {email}", email);
            }
        }

        public async Task SendWelcomeAsync(string email, string fullName)
        {
            try
            {                
                _logger.LogInformation("Enviando Email de bienvenida a {email}: {fullName}", email, fullName);

                using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
                {
                    Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
                    EnableSsl = _smtpSettings.EnableSsl
                };

                var message = new MailMessage(_smtpSettings.FromEmail, email)
                {
                    Subject = "¡Bienvenido a la Universidad!",
                    Body = $@"
                        <h2>¡Bienvenido {fullName}!</h2>
                        <p>Te damos la bienvenida a nuestra universidad. Te has registrado correctamente en nuestro sistema.</p>
                        <p>Esperamos que tengas una excelente experiencia académica con nosotros.</p>
                        <br>
                        <p>Saludos,<br>{_smtpSettings.FromName}</p>
                    ",
                    IsBodyHtml = true
                };

                message.From = new MailAddress(_smtpSettings.FromEmail, _smtpSettings.FromName);
                await smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email de bienvenida enviado exitosamente a {email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando Email a {email}", email);
            }
        }
    }
}
