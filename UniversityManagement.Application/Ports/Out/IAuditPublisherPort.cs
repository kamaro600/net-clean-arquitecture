using UniversityManagement.Application.DTOs.Messages;

namespace UniversityManagement.Application.Ports.Out;

/// <summary>
/// Puerto para el servicio de auditoría
/// </summary>
public interface IAuditPublisherPort
{
    Task PublishAuditEventAsync(AuditEventMessage auditEvent);
    Task PublishEnrollmentAuditAsync(string studentId, string careerId, string action, string? additionalData = null);
    Task PublishStudentAuditAsync(string studentId, string action, string? oldValues = null, string? newValues = null);
    Task PublishBulkAuditEventsAsync(IEnumerable<AuditEventMessage> auditEvents);
}