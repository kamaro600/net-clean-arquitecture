namespace UniversityManagement.Domain.Models;

/// <summary>
/// Entidad para auditoría de eventos del sistema
/// </summary>
public class AuditLog
{
    public Guid Id { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string EntityName { get; private set; } = string.Empty;
    public string EntityId { get; private set; } = string.Empty;
    public string Action { get; private set; } = string.Empty;
    public string UserId { get; private set; } = string.Empty;
    public string UserName { get; private set; } = string.Empty;
    public DateTime Timestamp { get; private set; }
    public string OldValues { get; private set; } = string.Empty;
    public string NewValues { get; private set; } = string.Empty;
    public string AdditionalData { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;

    private AuditLog() { } // Para EF Core

    public AuditLog(
        string eventType,
        string entityName,
        string entityId,
        string action,
        string userId,
        string userName,
        string? oldValues = null,
        string? newValues = null,
        string? additionalData = null,
        string? ipAddress = null,
        string? userAgent = null)
    {
        Id = Guid.NewGuid();
        EventType = eventType;
        EntityName = entityName;
        EntityId = entityId;
        Action = action;
        UserId = userId;
        UserName = userName;
        Timestamp = DateTime.UtcNow;
        OldValues = oldValues ?? string.Empty;
        NewValues = newValues ?? string.Empty;
        AdditionalData = additionalData ?? string.Empty;
        IpAddress = ipAddress ?? string.Empty;
        UserAgent = userAgent ?? string.Empty;
    }

    public static AuditLog CreateEnrollmentAudit(
        string studentId,
        string careerId,
        string action,
        string userId = "System",
        string userName = "System",
        string? additionalData = null)
    {
        return new AuditLog(
            eventType: "Enrollment",
            entityName: "StudentCareer",
            entityId: $"{studentId}-{careerId}",
            action: action,
            userId: userId,
            userName: userName,
            additionalData: additionalData
        );
    }

    public static AuditLog CreateStudentAudit(
        string studentId,
        string action,
        string? oldValues = null,
        string? newValues = null,
        string userId = "System",
        string userName = "System")
    {
        return new AuditLog(
            eventType: "Student",
            entityName: "Student",
            entityId: studentId,
            action: action,
            userId: userId,
            userName: userName,
            oldValues: oldValues,
            newValues: newValues
        );
    }
}