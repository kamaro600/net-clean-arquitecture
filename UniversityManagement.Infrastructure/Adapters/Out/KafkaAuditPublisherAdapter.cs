using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UniversityManagement.Application.DTOs.Messages;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Infrastructure.Configuration;

namespace UniversityManagement.Infrastructure.Adapters.Out;

/// <summary>
/// Adaptador para publicar eventos de auditoría en Kafka
/// </summary>
public class KafkaAuditPublisherAdapter : IAuditPublisherPort, IDisposable
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaAuditPublisherAdapter> _logger;
    private readonly IProducer<string, string> _producer;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaAuditPublisherAdapter(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaAuditPublisherAdapter> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;

        var config = new ProducerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            SecurityProtocol = SecurityProtocol.Plaintext,
            MessageTimeoutMs = _kafkaSettings.MessageTimeoutMs,
            RetryBackoffMs = _kafkaSettings.RetryBackoffMs,
            MessageSendMaxRetries = _kafkaSettings.Retries,
            Acks = Acks.All,
            EnableIdempotence = _kafkaSettings.EnableIdempotence,
            ClientId = $"university-audit-producer-{Environment.MachineName}-{Guid.NewGuid():N}"
        };

        // Configurar SASL si es necesario
        if (!string.IsNullOrEmpty(_kafkaSettings.SaslUsername))
        {
            config.SaslMechanism = Enum.Parse<SaslMechanism>(_kafkaSettings.SaslMechanism);
            config.SaslUsername = _kafkaSettings.SaslUsername;
            config.SaslPassword = _kafkaSettings.SaslPassword;
        }

        _producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Error}", e.Reason))
            .SetLogHandler((_, log) => _logger.LogDebug("Kafka log: {Message}", log.Message))
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }

    public async Task PublishAuditEventAsync(AuditEventMessage auditEvent)
    {
        try
        {
            var messageKey = $"{auditEvent.EntityName}:{auditEvent.EntityId}";
            var messageValue = JsonSerializer.Serialize(auditEvent, _jsonOptions);

            var message = new Message<string, string>
            {
                Key = messageKey,
                Value = messageValue,
                Headers = new Headers
                {
                    { "eventType", System.Text.Encoding.UTF8.GetBytes(auditEvent.EventType) },
                    { "timestamp", System.Text.Encoding.UTF8.GetBytes(auditEvent.Timestamp.ToString("O")) },
                    { "correlationId", System.Text.Encoding.UTF8.GetBytes(auditEvent.CorrelationId) },
                    { "source", System.Text.Encoding.UTF8.GetBytes(auditEvent.Source) }
                }
            };

            var result = await _producer.ProduceAsync(_kafkaSettings.AuditTopic, message);

            _logger.LogInformation(
                "Published audit event to Kafka. Topic: {Topic}, Partition: {Partition}, Offset: {Offset}, Key: {Key}",
                result.Topic, result.Partition, result.Offset, messageKey);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to publish audit event to Kafka for {EntityName}:{EntityId}", 
                auditEvent.EntityName, auditEvent.EntityId);
            
            // Intentar enviar a dead letter topic
            await TrySendToDeadLetterTopicAsync(auditEvent, ex.Message);
            throw;
        }
    }

    public async Task PublishEnrollmentAuditAsync(string studentId, string careerId, string action, string? additionalData = null)
    {
        var auditEvent = new AuditEventMessage
        {
            EventType = "Enrollment",
            EntityName = "StudentCareer",
            EntityId = $"{studentId}-{careerId}",
            Action = action,
            UserId = "System",
            UserName = "System",
            AdditionalData = additionalData ?? string.Empty
        };

        await PublishAuditEventAsync(auditEvent);
    }

    public async Task PublishStudentAuditAsync(string studentId, string action, string? oldValues = null, string? newValues = null)
    {
        var auditEvent = new AuditEventMessage
        {
            EventType = "Student",
            EntityName = "Student",
            EntityId = studentId,
            Action = action,
            UserId = "System",
            UserName = "System",
            OldValues = oldValues ?? string.Empty,
            NewValues = newValues ?? string.Empty
        };

        await PublishAuditEventAsync(auditEvent);
    }

    public async Task PublishBulkAuditEventsAsync(IEnumerable<AuditEventMessage> auditEvents)
    {
        var tasks = auditEvents.Select(PublishAuditEventAsync);
        await Task.WhenAll(tasks);
    }

    private async Task TrySendToDeadLetterTopicAsync(AuditEventMessage auditEvent, string errorMessage)
    {
        try
        {
            auditEvent.AdditionalData = $"Error: {errorMessage}. Original Data: {auditEvent.AdditionalData}";
            
            var messageKey = $"dead-letter:{auditEvent.EntityName}:{auditEvent.EntityId}";
            var messageValue = JsonSerializer.Serialize(auditEvent, _jsonOptions);

            var deadLetterMessage = new Message<string, string>
            {
                Key = messageKey,
                Value = messageValue,
                Headers = new Headers
                {
                    { "originalTopic", System.Text.Encoding.UTF8.GetBytes(_kafkaSettings.AuditTopic) },
                    { "error", System.Text.Encoding.UTF8.GetBytes(errorMessage) },
                    { "timestamp", System.Text.Encoding.UTF8.GetBytes(DateTime.UtcNow.ToString("O")) }
                }
            };

            await _producer.ProduceAsync(_kafkaSettings.DeadLetterTopic, deadLetterMessage);
            
            _logger.LogWarning("Sent failed audit event to dead letter topic for {EntityName}:{EntityId}", 
                auditEvent.EntityName, auditEvent.EntityId);
        }
        catch (Exception deadLetterEx)
        {
            _logger.LogError(deadLetterEx, "Failed to send audit event to dead letter topic for {EntityName}:{EntityId}", 
                auditEvent.EntityName, auditEvent.EntityId);
        }
    }

    public void Dispose()
    {
        try
        {
            _producer?.Flush(TimeSpan.FromSeconds(10));
            _producer?.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error disposing Kafka producer");
        }
    }
}