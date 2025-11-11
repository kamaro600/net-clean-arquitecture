using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UniversityManagement.Infrastructure.Configuration;

namespace UniversityManagement.Infrastructure.Services;

/// <summary>
/// Servicio para configurar automáticamente los topics de Kafka al iniciar la aplicación
/// </summary>
public class KafkaTopicSetupService : IHostedService
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaTopicSetupService> _logger;

    public KafkaTopicSetupService(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaTopicSetupService> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Configurando topics de Kafka");

        try
        {
            // Agregar timeout para evitar bloqueos indefinidos
            using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            using var combinedCts = CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken, timeoutCts.Token);

            await CreateTopicsIfNotExistAsync(combinedCts.Token);
            _logger.LogInformation("Topics de Kafka configurados correctamente");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning("Configuración de topics de Kafka cancelada durante el shutdown");
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Timeout en configuración de topics de Kafka - continuando sin bloquear la aplicación");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al configurar topics de Kafka: {Error}", ex.Message);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Servicio de configuración de Kafka detenido");
        return Task.CompletedTask;
    }

    private async Task CreateTopicsIfNotExistAsync(CancellationToken cancellationToken)
    {
        var adminConfig = new AdminClientConfig
        {
            BootstrapServers = _kafkaSettings.BootstrapServers,
            SecurityProtocol = SecurityProtocol.Plaintext
        };

        // Configurar SASL si es necesario
        if (!string.IsNullOrEmpty(_kafkaSettings.SaslUsername))
        {
            adminConfig.SaslMechanism = Enum.Parse<SaslMechanism>(_kafkaSettings.SaslMechanism);
            adminConfig.SaslUsername = _kafkaSettings.SaslUsername;
            adminConfig.SaslPassword = _kafkaSettings.SaslPassword;
        }

        using var adminClient = new AdminClientBuilder(adminConfig).Build();

        try
        {
            // Verificar topics existentes
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();

            var topicsToCreate = new List<TopicSpecification>();

            // Topic principal de auditoría
            if (!existingTopics.Contains(_kafkaSettings.AuditTopic))
            {
                topicsToCreate.Add(new TopicSpecification
                {
                    Name = _kafkaSettings.AuditTopic,
                    NumPartitions = 3, // Múltiples particiones para mayor throughput
                    ReplicationFactor = 1, 
                    Configs = new Dictionary<string, string>
                    {
                        { "retention.ms", "604800000" }, // 7 días
                        { "compression.type", "snappy" },
                        { "max.message.bytes", "1048576" } // 1MB
                    }
                });

                _logger.LogInformation("Topic '{Topic}' será creado", _kafkaSettings.AuditTopic);
            }

            // Topic de Dead Letter Queue
            if (!existingTopics.Contains(_kafkaSettings.DeadLetterTopic))
            {
                topicsToCreate.Add(new TopicSpecification
                {
                    Name = _kafkaSettings.DeadLetterTopic,
                    NumPartitions = 1, // Una sola partición para DLQ
                    ReplicationFactor = 1,
                    Configs = new Dictionary<string, string>
                    {
                        { "retention.ms", "2592000000" }, // 30 días para investigar errores
                        { "compression.type", "snappy" }
                    }
                });

                _logger.LogInformation("Topic '{Topic}' será creado", _kafkaSettings.DeadLetterTopic);
            }

            // Crear topics si hay alguno pendiente
            if (topicsToCreate.Any())
            {
                await adminClient.CreateTopicsAsync(topicsToCreate);
                
                foreach (var topic in topicsToCreate)
                {
                    _logger.LogInformation("Topic '{Topic}' creado exitosamente con {Partitions} particiones", 
                        topic.Name, topic.NumPartitions);
                }
            }
            else
            {
                _logger.LogInformation("Todos los topics ya existen. No se requiere crear nuevos topics.");
            }

            // Verificar que los topics fueron creados correctamente
            await VerifyTopicsAsync(adminClient, cancellationToken);
        }
        catch (CreateTopicsException ex)
        {
            _logger.LogError("Error al crear topics: {Error}", ex.Message);
            
            foreach (var result in ex.Results)
            {
                if (result.Error.Code != ErrorCode.TopicAlreadyExists)
                {
                    _logger.LogError("Error creando topic '{Topic}': {Error}", 
                        result.Topic, result.Error.Reason);
                }
                else
                {
                    _logger.LogInformation("Topic '{Topic}' ya existe", result.Topic);
                }
            }
        }
    }

    private async Task VerifyTopicsAsync(IAdminClient adminClient, CancellationToken cancellationToken)
    {
        try
        {
            // Esperar un poco para que los topics se propaguen
            await Task.Delay(2000, cancellationToken);

            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(10));
            var topics = metadata.Topics.ToDictionary(t => t.Topic, t => t);

            // Verificar topic principal
            if (topics.TryGetValue(_kafkaSettings.AuditTopic, out var auditTopic))
            {
                _logger.LogInformation("Topic '{Topic}' verificado - Particiones: {Partitions}", 
                    _kafkaSettings.AuditTopic, auditTopic.Partitions.Count);
            }
            else
            {
                _logger.LogWarning(" Topic '{Topic}' no encontrado después de la creación", 
                    _kafkaSettings.AuditTopic);
            }

            // Verificar DLQ
            if (topics.TryGetValue(_kafkaSettings.DeadLetterTopic, out var dlqTopic))
            {
                _logger.LogInformation("Topic '{Topic}' verificado - Particiones: {Partitions}", 
                    _kafkaSettings.DeadLetterTopic, dlqTopic.Partitions.Count);
            }
            else
            {
                _logger.LogWarning("Topic '{Topic}' no encontrado después de la creación", 
                    _kafkaSettings.DeadLetterTopic);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudo verificar los topics: {Error}", ex.Message);
        }
    }
}