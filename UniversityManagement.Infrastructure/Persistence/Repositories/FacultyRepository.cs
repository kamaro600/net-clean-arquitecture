using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de facultades
/// Implementa tanto la interfaz de dominio como el puerto de aplicación
/// </summary>
public class FacultyRepository : IFacultyRepository
{
    private readonly UniversityDbContext _context;

    public FacultyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Faculty> CreateAsync(Faculty faculty)
    {
        _context.Faculties.Add(faculty);
        await _context.SaveChangesAsync();
        return faculty;
    }

    public async Task<Faculty?> GetByIdAsync(int id)
    {
        return await _context.Faculties
            .Include(f => f.Careers)
            .FirstOrDefaultAsync(f => f.FacultadId == id);
    }

    public async Task<Faculty?> GetByNameAsync(string name)
    {
        return await _context.Faculties
            .FirstOrDefaultAsync(f => f.Nombre == name);
    }

    public async Task<Faculty> UpdateAsync(Faculty faculty)
    {
        _context.Faculties.Update(faculty);
        await _context.SaveChangesAsync();
        return faculty;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var faculty = await _context.Faculties.FindAsync(id);
        if (faculty == null)
            return false;

        faculty.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<Faculty> Faculties, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Faculties
            .Include(f => f.Careers)
            .Where(f => f.Activo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(f => f.Nombre.Contains(searchTerm) || 
                                   f.Descripcion!.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();
        
        var faculties = await query
            .OrderBy(f => f.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (faculties, totalCount);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Faculties.AnyAsync(f => f.Nombre == name && f.Activo);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Faculties.AnyAsync(f => f.FacultadId == id && f.Activo);
    }

    // Métodos de la interfaz de dominio
    public async Task<IEnumerable<Faculty>> GetAllAsync()
    {
        return await _context.Faculties
            .Include(f => f.Careers)
            .Where(f => f.Activo)
            .OrderBy(f => f.Nombre)
            .ToListAsync();
    }
}