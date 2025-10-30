using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using UniversityManagement.Application.Ports.Out;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UniversityManagement.Infrastructure.Adapters.Out
{
    public class EmailNotificationAdapter : IEmailNotificationPort
    {
        private readonly ILogger<EmailNotificationAdapter> _logger;
        private readonly SmtpClient _smtpClient;

        public EmailNotificationAdapter(ILogger<EmailNotificationAdapter> logger, SmtpClient smtpClient)
        {
            _logger = logger;
            _smtpClient = smtpClient;
        }

        public async Task SendEnrollmentCancellation(string email, string ownerName, string course, string enrollmentDate)
        {
            try
            {                
                _logger.LogInformation("Enviando Email a {email}: {Message}", email, ownerName);

                var message = new MailMessage("noreply@academic.com", email)
                {
                    Subject = "Matricula - Gestion universitaria",
                    Body = $"Hola {ownerName}, tu cancelacion de matricula para {course} se completo el dia  {enrollmentDate}."
                };

                await _smtpClient.SendMailAsync(message);

                await Task.CompletedTask;
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

                var message = new MailMessage("noreply@academic.com", email)
                {
                    Subject = "Matricula - Gestion universitaria",
                    Body = $"Hola {ownerName}, tu inscripcion para {course} se completo el dia  {enrollmentDate}."
                };

                await _smtpClient.SendMailAsync(message);

                await Task.CompletedTask;
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
                _logger.LogInformation("Enviando Email a {email}: {nombre}", email, nombre);

                var message = new MailMessage("noreply@academic.com", email)
                {
                    Subject = "Actualizacion - Gestion universitaria",
                    Body = $"Hola {nombre}, ocurrio un evento."
                };

                await _smtpClient.SendMailAsync(message);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando SMS a {email}", email);
            }
        }

        public async Task SendWelcomeAsync(string email, string fullName)
        {
            try
            {                
                _logger.LogInformation("Enviando Email a {email}: {fullName}", email, fullName);

                var message = new MailMessage("noreply@academic.com", email)
                {
                    Subject = "Inscripcion - Gestion universitaria",
                    Body = $"Hola {fullName}, se ha registrado correctamente."
                };

                await _smtpClient.SendMailAsync(message);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando Email a {email}", email);
            }
        }
    }
}
