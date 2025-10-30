using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de carreras
/// Implementa tanto la interfaz de dominio como el puerto de aplicación
/// </summary>
public class CareerRepository : ICareerRepository
{
    private readonly UniversityDbContext _context;

    public CareerRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<Career> CreateAsync(Career career)
    {
        _context.Careers.Add(career);
        await _context.SaveChangesAsync();
        return career;
    }

    public async Task<Career?> GetByIdAsync(int id)
    {
        return await _context.Careers
            .Include(c => c.Faculty)
            .FirstOrDefaultAsync(c => c.CarreraId == id);
    }

    public async Task<Career?> GetByNameAsync(string name)
    {
        return await _context.Careers
            .FirstOrDefaultAsync(c => c.Nombre == name);
    }

    public async Task<Career> UpdateAsync(Career career)
    {
        _context.Careers.Update(career);
        await _context.SaveChangesAsync();
        return career;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var career = await _context.Careers.FindAsync(id);
        if (career == null)
            return false;

        career.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<(List<Career> Careers, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.Careers
            .Include(c => c.Faculty)
            .Where(c => c.Activo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(c => c.Nombre.Contains(searchTerm) || 
                                   c.Descripcion!.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();
        
        var careers = await query
            .OrderBy(c => c.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (careers, totalCount);
    }

    public async Task<(List<Career> Careers, int TotalCount)> GetByFacultyPagedAsync(int facultyId, int page, int pageSize)
    {
        var query = _context.Careers
            .Include(c => c.Faculty)
            .Where(c => c.FacultadId == facultyId && c.Activo);

        var totalCount = await query.CountAsync();
        
        var careers = await query
            .OrderBy(c => c.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (careers, totalCount);
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        return await _context.Careers.AnyAsync(c => c.Nombre == name && c.Activo);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Careers.AnyAsync(c => c.CarreraId == id && c.Activo);
    }

    // Métodos de la interfaz de dominio
    public async Task<IEnumerable<Career>> GetAllAsync()
    {
        return await _context.Careers
            .Include(c => c.Faculty)
            .Where(c => c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }

    public async Task<IEnumerable<Career>> GetByFacultyIdAsync(int facultyId)
    {
        return await _context.Careers
            .Include(c => c.Faculty)
            .Where(c => c.FacultadId == facultyId && c.Activo)
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }
}