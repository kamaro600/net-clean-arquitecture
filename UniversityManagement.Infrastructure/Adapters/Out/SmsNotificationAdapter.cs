using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Domain.Models.ValueObjects;

namespace UniversityManagement.Infrastructure.Adapters.Out
{
    public class SmsNotificationAdapter : ISmsNotificationPort
    {
        private readonly ILogger<SmsNotificationAdapter> _logger;
        // private readonly TwilioRestClient _twilioClient; // Ejemplo si usaras Twilio

        public SmsNotificationAdapter(ILogger<SmsNotificationAdapter> logger)
        {
            _logger = logger;
        }
        public async Task SendMessageAsync(string phone, string message)
        {
            try
            {
                _logger.LogInformation("Enviando Sms a {phone}: {Message}", phone, message);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enviando Sms a {phone}", phone);
            }
        }
    }
}
