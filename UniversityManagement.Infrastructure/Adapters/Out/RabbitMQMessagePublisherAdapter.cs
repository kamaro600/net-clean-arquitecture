using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using UniversityManagement.Application.DTOs.Messages;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Infrastructure.Configuration;
using UniversityManagement.Infrastructure.Services;

namespace UniversityManagement.Infrastructure.Adapters.Out;

/// <summary>
/// Adaptador para publicar mensajes en RabbitMQ (Productor)
/// </summary>
public class RabbitMQMessagePublisherAdapter : IMessagePublisherPort
{
    private readonly RabbitMQConnectionService _connectionService;
    private readonly RabbitMQSettings _settings;
    private readonly ILogger<RabbitMQMessagePublisherAdapter> _logger;

    public RabbitMQMessagePublisherAdapter(
        RabbitMQConnectionService connectionService,
        IOptions<RabbitMQSettings> settings,
        ILogger<RabbitMQMessagePublisherAdapter> logger)
    {
        _connectionService = connectionService;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task PublishEnrollmentNotificationAsync(EnrollmentNotificationMessage message)
    {
        try
        {
            message.NotificationType = "Enrollment";
            await PublishMessageAsync(message, _settings.EnrollmentRoutingKey);
            _logger.LogInformation("Published enrollment notification message for student {StudentEmail}", message.StudentEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish enrollment notification message for student {StudentEmail}", message.StudentEmail);
            throw;
        }
    }

    public async Task PublishUnenrollmentNotificationAsync(EnrollmentNotificationMessage message)
    {
        try
        {
            message.NotificationType = "Unenrollment";
            await PublishMessageAsync(message, _settings.UnenrollmentRoutingKey);
            _logger.LogInformation("Published unenrollment notification message for student {StudentEmail}", message.StudentEmail);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish unenrollment notification message for student {StudentEmail}", message.StudentEmail);
            throw;
        }
    }

    private async Task PublishMessageAsync(EnrollmentNotificationMessage message, string routingKey)
    {
        await Task.Run(() =>
        {
            var channel = _connectionService.GetChannel();
            
            var messageBody = JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            var body = Encoding.UTF8.GetBytes(messageBody);

            var properties = channel.CreateBasicProperties();
            properties.Persistent = true;
            properties.MessageId = message.MessageId;
            properties.Timestamp = new AmqpTimestamp(((DateTimeOffset)message.CreatedAt).ToUnixTimeSeconds());
            properties.ContentType = "application/json";

            channel.BasicPublish(
                exchange: _settings.ExchangeName,
                routingKey: routingKey,
                basicProperties: properties,
                body: body
            );
        });
    }
}