using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuditController : ControllerBase
{
    private readonly UniversityDbContext _context;

    public AuditController(UniversityDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Obtiene todos los registros de auditoría
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAuditLogs()
    {
        try
        {
            var auditLogs = await _context.AuditLogs
                .OrderByDescending(a => a.Timestamp)
                .Take(50)
                .ToListAsync();

            return Ok(new
            {
                TotalRecords = auditLogs.Count,
                Records = auditLogs.Select(a => new
                {
                    a.Id,
                    a.EventType,
                    a.EntityName,
                    a.EntityId,
                    a.Action,
                    a.UserId,
                    a.UserName,
                    a.Timestamp,
                    a.AdditionalData
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener logs de auditoría: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene registros de auditoría por tipo de evento
    /// </summary>
    [HttpGet("by-event/{eventType}")]
    public async Task<IActionResult> GetAuditLogsByEvent(string eventType)
    {
        try
        {
            var auditLogs = await _context.AuditLogs
                .Where(a => a.EventType == eventType)
                .OrderByDescending(a => a.Timestamp)
                .Take(20)
                .ToListAsync();

            return Ok(auditLogs);
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener logs por evento: {ex.Message}");
        }
    }

    /// <summary>
    /// Obtiene estadísticas de auditoría
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetAuditStats()
    {
        try
        {
            var totalLogs = await _context.AuditLogs.CountAsync();
            var eventTypes = await _context.AuditLogs
                .GroupBy(a => a.EventType)
                .Select(g => new { EventType = g.Key, Count = g.Count() })
                .ToListAsync();

            var recentLogs = await _context.AuditLogs
                .Where(a => a.Timestamp >= DateTime.UtcNow.AddHours(-1))
                .CountAsync();

            return Ok(new
            {
                TotalLogs = totalLogs,
                EventTypes = eventTypes,
                RecentLogsLastHour = recentLogs,
                LastUpdated = DateTime.UtcNow
            });
        }
        catch (Exception ex)
        {
            return BadRequest($"Error al obtener estadísticas: {ex.Message}");
        }
    }
}