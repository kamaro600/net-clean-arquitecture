using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de auditoría
/// </summary>
public class AuditLogRepository : IAuditLogRepository
{
    private readonly UniversityDbContext _context;

    public AuditLogRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<AuditLog> AddAsync(AuditLog auditLog)
    {
        _context.AuditLogs.Add(auditLog);
        await _context.SaveChangesAsync();
        return auditLog;
    }

    public async Task AddBulkAsync(IEnumerable<AuditLog> auditLogs)
    {
        _context.AuditLogs.AddRange(auditLogs);
        await _context.SaveChangesAsync();
    }

    public async Task<AuditLog?> GetByIdAsync(Guid id)
    {
        return await _context.AuditLogs
            .FirstOrDefaultAsync(a => a.Id == id);
    }

    public async Task<IEnumerable<AuditLog>> GetByEntityAsync(string entityName, string entityId)
    {
        return await _context.AuditLogs
            .Where(a => a.EntityName == entityName && a.EntityId == entityId)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByUserAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.AuditLogs
            .Where(a => a.UserId == userId);

        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate.Value);

        return await query
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByEventTypeAsync(string eventType, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.AuditLogs
            .Where(a => a.EventType == eventType);

        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate.Value);

        return await query
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<IEnumerable<AuditLog>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
    {
        return await _context.AuditLogs
            .Where(a => a.Timestamp >= fromDate && a.Timestamp <= toDate)
            .OrderByDescending(a => a.Timestamp)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _context.AuditLogs
            .AnyAsync(a => a.Id == id);
    }
}