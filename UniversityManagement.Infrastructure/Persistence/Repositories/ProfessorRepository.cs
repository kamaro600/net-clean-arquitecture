using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de profesores
/// Implementa tanto la interfaz de dominio como el puerto de aplicación
/// </summary>
public class ProfessorRepository : IProfessorRepository
{
    private readonly UniversityDbContext _context;

    public ProfessorRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Professor> CreateAsync(Professor professor)
    {
        _context.Professors.Add(professor);
        await _context.SaveChangesAsync();
        return professor;
    }

    public async Task<Professor?> GetByIdAsync(int id)
    {
        return await _context.Professors
            .FirstOrDefaultAsync(p => p.ProfesorId == id);
    }

    public async Task<Professor?> GetByDniAsync(string dni)
    {
        return await _context.Professors
            .FirstOrDefaultAsync(p => p.Dni == dni);
    }

    public async Task<Professor?> GetByEmailAsync(string email)
    {
        return await _context.Professors
            .FirstOrDefaultAsync(p => p.Email == email);
    }

    public async Task<Professor> UpdateAsync(Professor professor)
    {
        _context.Professors.Update(professor);
        await _context.SaveChangesAsync();
        return professor;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var professor = await _context.Professors.FindAsync(id);
        if (professor == null)
            return false;

        professor.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<Professor> Professors, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Professors
            .Where(p => p.Activo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(p => p.Nombre.Contains(searchTerm) || 
                                   p.Apellido.Contains(searchTerm) ||
                                   p.Especialidad!.Contains(searchTerm) ||
                                   p.Email.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();
        
        var professors = await query
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (professors, totalCount);
    }

    public async Task<(List<Professor> Professors, int TotalCount)> GetBySpecialtyPagedAsync(string specialty, int page, int pageSize)
    {
        var query = _context.Professors
            .Where(p => p.Especialidad == specialty && p.Activo);

        var totalCount = await query.CountAsync();
        
        var professors = await query
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (professors, totalCount);
    }

    public async Task<bool> ExistsByDniAsync(string dni)
    {
        return await _context.Professors.AnyAsync(p => p.Dni == dni && p.Activo);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Professors.AnyAsync(p => p.Email == email && p.Activo);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Professors.AnyAsync(p => p.ProfesorId == id && p.Activo);
    }

    // Métodos de la interfaz de dominio
    public async Task<IEnumerable<Professor>> GetAllAsync()
    {
        return await _context.Professors
            .Where(p => p.Activo)
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Professor>> GetBySpecialtyAsync(string specialty)
    {
        return await _context.Professors
            .Where(p => p.Especialidad == specialty && p.Activo)
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Professor>> GetProfessorsByCareerId(int careerId)
    {
        return await _context.Professors
            .Where(p => p.ProfessorCareers.Any(pc => pc.CarreraId == careerId && pc.Activo) && p.Activo)
            .Include(p => p.ProfessorCareers)
            .ThenInclude(pc => pc.Career)
            .OrderBy(p => p.Apellido)
            .ThenBy(p => p.Nombre)
            .ToListAsync();
    }
}