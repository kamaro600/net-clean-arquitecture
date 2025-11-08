using Confluent.Kafka;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using UniversityManagement.Infrastructure.Configuration;

namespace UniversityManagement.Infrastructure.HealthChecks;

/// <summary>
/// Health check para verificar la conectividad con Kafka
/// </summary>
public class KafkaHealthCheck : IHealthCheck
{
    private readonly KafkaSettings _kafkaSettings;
    private readonly ILogger<KafkaHealthCheck> _logger;

    public KafkaHealthCheck(
        IOptions<KafkaSettings> kafkaSettings,
        ILogger<KafkaHealthCheck> logger)
    {
        _kafkaSettings = kafkaSettings.Value;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context, 
        CancellationToken cancellationToken = default)
    {
        try
        {
            var config = new AdminClientConfig
            {
                BootstrapServers = _kafkaSettings.BootstrapServers,
                SecurityProtocol = SecurityProtocol.Plaintext
            };

            using var adminClient = new AdminClientBuilder(config).Build();
            
            // Intentar obtener metadatos con timeout corto
            var metadata = adminClient.GetMetadata(TimeSpan.FromSeconds(5));
            
            var data = new Dictionary<string, object>
            {
                ["broker_count"] = metadata.Brokers.Count,
                ["topic_count"] = metadata.Topics.Count,
                ["bootstrap_servers"] = _kafkaSettings.BootstrapServers
            };

            // Verificar que los topics de auditoría existen
            var existingTopics = metadata.Topics.Select(t => t.Topic).ToHashSet();
            var requiredTopics = new[] { _kafkaSettings.AuditTopic, _kafkaSettings.DeadLetterTopic };
            var missingTopics = requiredTopics.Where(t => !existingTopics.Contains(t)).ToArray();

            if (missingTopics.Any())
            {
                data["missing_topics"] = missingTopics;
                return HealthCheckResult.Degraded(
                    $"Kafka está disponible pero faltan topics: {string.Join(", ", missingTopics)}", 
                    data: data);
            }

            data["audit_topics"] = requiredTopics;
            return HealthCheckResult.Healthy("Kafka está disponible y todos los topics existen", data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en health check de Kafka: {Error}", ex.Message);
            
            return HealthCheckResult.Unhealthy(
                $"Kafka no está disponible: {ex.Message}", 
                ex, 
                new Dictionary<string, object> 
                { 
                    ["bootstrap_servers"] = _kafkaSettings.BootstrapServers,
                    ["error"] = ex.Message
                });
        }
    }
}