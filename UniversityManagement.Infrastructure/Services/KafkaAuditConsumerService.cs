using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UniversityManagement.Application.DTOs.Messages;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Configuration;

namespace UniversityManagement.Infrastructure.Services;

/// <summary>
/// Servicio consumidor de Kafka para procesar eventos de auditoría
/// </summary>
public class KafkaAuditConsumerService : BackgroundService
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<KafkaAuditConsumerService> _logger;
    private readonly IConsumer<string, string> _consumer;
    private readonly JsonSerializerOptions _jsonOptions;

    public KafkaAuditConsumerService(
        IOptions<KafkaSettings> kafkaSettings,
        IServiceScopeFactory serviceScopeFactory,
        ILogger<KafkaAuditConsumerService> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;

        var config = new ConsumerConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            GroupId = _kafkaSettings.GroupId,
            SecurityProtocol = SecurityProtocol.Plaintext,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = _kafkaSettings.EnableAutoCommit,
            SessionTimeoutMs = _kafkaSettings.SessionTimeoutMs,
            MaxPollIntervalMs = _kafkaSettings.MaxPollIntervalMs,
            ClientId = $"university-audit-consumer-{Environment.MachineName}-{Guid.NewGuid():N}"
        };

        // Configurar SASL si es necesario
        if (!string.IsNullOrEmpty(_kafkaSettings.SaslUsername))
        {
            config.SaslMechanism = Enum.Parse<SaslMechanism>(_kafkaSettings.SaslMechanism);
            config.SaslUsername = _kafkaSettings.SaslUsername;
            config.SaslPassword = _kafkaSettings.SaslPassword;
        }

        _consumer = new ConsumerBuilder<string, string>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka consumer error: {Error}", e.Reason))
            .SetLogHandler((_, log) => _logger.LogDebug("Kafka consumer log: {Message}", log.Message))
            .Build();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            // Esperar un poco antes de iniciar para asegurar que Kafka esté listo
            await Task.Delay(2000, stoppingToken);
            
            _consumer.Subscribe(_kafkaSettings.AuditTopic);
            _logger.LogInformation("Kafka Audit Consumer started. Subscribed to topic: {Topic}", _kafkaSettings.AuditTopic);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = _consumer.Consume(TimeSpan.FromMilliseconds(100));
                    
                    if (consumeResult?.Message != null)
                    {
                        await ProcessAuditEventAsync(consumeResult);
                        
                        // Commit manualmente si auto-commit está deshabilitado
                        if (!_kafkaSettings.EnableAutoCommit)
                        {
                            _consumer.Commit(consumeResult);
                        }
                    }
                    else if (consumeResult == null)
                    {
                        // No hay mensajes, hacer una pequeña pausa para evitar consumo excesivo de CPU
                        await Task.Delay(50, stoppingToken);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming message from Kafka");
                    await Task.Delay(1000, stoppingToken); // Esperar antes de reintentar
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Kafka Audit Consumer operation was cancelled");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error in Kafka Audit Consumer");
                    await Task.Delay(5000, stoppingToken); // Esperar antes de reintentar
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fatal error in Kafka Audit Consumer");
        }
        finally
        {
            try
            {
                _consumer.Close();
                _consumer.Dispose();
                _logger.LogInformation("Kafka Audit Consumer stopped");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing Kafka consumer");
            }
        }
    }

    private async Task ProcessAuditEventAsync(ConsumeResult<string, string> consumeResult)
    {
        try
        {
            var message = consumeResult.Message;
            _logger.LogDebug("Processing audit event. Key: {Key}, Partition: {Partition}, Offset: {Offset}",
                message.Key, consumeResult.Partition, consumeResult.Offset);

            // Deserializar el mensaje
            var auditEventMessage = JsonSerializer.Deserialize<AuditEventMessage>(message.Value, _jsonOptions);
            
            if (auditEventMessage == null)
            {
                _logger.LogWarning("Failed to deserialize audit event message. Key: {Key}", message.Key);
                return;
            }

            // Crear entidad de auditoría
            var auditLog = new AuditLog(
                eventType: auditEventMessage.EventType,
                entityName: auditEventMessage.EntityName,
                entityId: auditEventMessage.EntityId,
                action: auditEventMessage.Action,
                userId: auditEventMessage.UserId,
                userName: auditEventMessage.UserName,
                oldValues: auditEventMessage.OldValues,
                newValues: auditEventMessage.NewValues,
                additionalData: auditEventMessage.AdditionalData,
                ipAddress: auditEventMessage.IpAddress,
                userAgent: auditEventMessage.UserAgent
            );

            // Guardar en base de datos usando scope
            using var scope = _serviceScopeFactory.CreateScope();
            var auditRepository = scope.ServiceProvider.GetRequiredService<IAuditLogRepository>();
            
            await auditRepository.AddAsync(auditLog);

            _logger.LogInformation(
                "Audit event processed and saved. EventType: {EventType}, Entity: {EntityName}:{EntityId}, Action: {Action}",
                auditEventMessage.EventType, auditEventMessage.EntityName, auditEventMessage.EntityId, auditEventMessage.Action);

            // Extraer headers para logging adicional
            if (message.Headers != null)
            {
                var correlationId = GetHeaderValue(message.Headers, "correlationId");
                var source = GetHeaderValue(message.Headers, "source");
                
                _logger.LogDebug("Additional info - CorrelationId: {CorrelationId}, Source: {Source}",
                    correlationId, source);
            }
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize audit event message. Key: {Key}, Value: {Value}",
                consumeResult.Message.Key, consumeResult.Message.Value);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audit event. Key: {Key}", consumeResult.Message.Key);
            throw; // Re-throw para que Kafka maneje el reintento
        }
    }

    private string GetHeaderValue(Headers headers, string key)
    {
        try
        {
            if (headers.TryGetLastBytes(key, out var headerValue))
            {
                return System.Text.Encoding.UTF8.GetString(headerValue);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error reading header {Key}", key);
        }
        
        return string.Empty;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping Kafka Audit Consumer...");
        await base.StopAsync(cancellationToken);
    }
}