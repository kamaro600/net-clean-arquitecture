using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

/// <summary>
/// Repositorio para gestión de logs de auditoría
/// </summary>
public interface IAuditLogRepository
{
    Task<AuditLog> AddAsync(AuditLog auditLog);
    Task AddBulkAsync(IEnumerable<AuditLog> auditLogs);
    Task<AuditLog?> GetByIdAsync(Guid id);
    Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId);
    Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<AuditLog>> GetByEventTypeAsync(string eventType, DateTime? fromDate = null, DateTime? toDate = null);
    Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate);
    Task<bool> ExistsAsync(Guid id);
}