namespace UniversityManagement.Infrastructure.Configuration;

/// <summary>
/// Configuración para Apache Kafka
/// </summary>
public class KafkaSettings
{
    public string BootstrapServers { get; set; } = "localhost:9092";
    public string SecurityProtocol { get; set; } = "PLAINTEXT";
    public string SaslMechanism { get; set; } = "PLAIN";
    public string SaslUsername { get; set; } = string.Empty;
    public string SaslPassword { get; set; } = string.Empty;
    
    // Topics
    public string AuditTopic { get; set; } = "university.audit.events";
    public string DeadLetterTopic { get; set; } = "university.audit.events.dlq";
    
    // Producer Settings
    public int MessageTimeoutMs { get; set; } = 5000;
    public int RetryBackoffMs { get; set; } = 1000;
    public int Retries { get; set; } = 3;
    public string Acks { get; set; } = "all";
    public bool EnableIdempotence { get; set; } = true;
    
    // Consumer Settings
    public string GroupId { get; set; } = "university-audit-consumer";
    public string AutoOffsetReset { get; set; } = "earliest";
    public bool EnableAutoCommit { get; set; } = false;
    public int SessionTimeoutMs { get; set; } = 30000;
    public int MaxPollIntervalMs { get; set; } = 300000;
}