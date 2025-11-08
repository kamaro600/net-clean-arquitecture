namespace UniversityManagement.Application.DTOs.Messages;

/// <summary>
/// Mensaje para eventos de auditoría a través de Kafka
/// </summary>
public class AuditEventMessage
{
    public string EventId { get; set; } = Guid.NewGuid().ToString();
    public string EventType { get; set; } = string.Empty;
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string OldValues { get; set; } = string.Empty;
    public string NewValues { get; set; } = string.Empty;
    public string AdditionalData { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString();
    public string Source { get; set; } = "UniversityManagement";
    public string Environment { get; set; } = "Development";
}