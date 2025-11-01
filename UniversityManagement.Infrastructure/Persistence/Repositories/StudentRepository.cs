using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Models;
using UniversityManagement.Infrastructure.Data;

namespace UniversityManagement.Infrastructure.Persistence.Repositories;

/// <summary>
/// Implementación del repositorio de estudiantes
/// </summary>
public class StudentRepository : IStudentRepository
{
    private readonly UniversityDbContext _context;

    public StudentRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Student>> GetAllAsync()
    {
        return await _context.Students
            .Where(s => s.Activo)
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .ToListAsync();
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        return await _context.Students
            .Include(s => s.StudentCareers)
            .ThenInclude(sc => sc.Career)
            .FirstOrDefaultAsync(s => s.EstudianteId == id);
    }

    public async Task<Student?> GetByDniAsync(string dni)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Dni == dni);
    }

    public async Task<Student?> GetByEmailAsync(string email)
    {
        return await _context.Students
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Student> CreateAsync(Student student)
    {
        _context.Students.Add(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        _context.Students.Update(student);
        await _context.SaveChangesAsync();
        return student;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null)
            return false;

        student.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByDniAsync(string dni)
    {
        return await _context.Students.AnyAsync(s => s.Dni == dni);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        return await _context.Students.AnyAsync(s => s.Email == email);
    }

    public async Task<IEnumerable<Student>> GetStudentsByCareerId(int careerId)
    {
        return await _context.Students
            .Where(s => s.StudentCareers.Any(sc => sc.CareerId == careerId && sc.IsActive))
            .Include(s => s.StudentCareers)
            .ThenInclude(sc => sc.Career)
            .ToListAsync();
    }

    // Métodos del puerto de aplicación (IStudentRepositoryPort)
    public async Task<(List<Student> Students, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null, bool? onlyActive = null)
    {
        var query = _context.Students.AsQueryable();

        // Filtrar por activos si se especifica
        if (onlyActive.HasValue)
        {
            query = query.Where(s => s.Activo == onlyActive.Value);
        }

        // Aplicar término de búsqueda
        if (!string.IsNullOrEmpty(searchTerm))
        {
            query = query.Where(s => s.Nombre.Contains(searchTerm) || 
                                   s.Apellido.Contains(searchTerm) ||
                                   s.Email.Contains(searchTerm) ||
                                   s.Dni.Contains(searchTerm));
        }

        var totalCount = await query.CountAsync();
        
        var students = await query
            .Include(s => s.StudentCareers)
            .ThenInclude(sc => sc.Career)
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }

    public async Task<(List<Student> Students, int TotalCount)> GetByCareerPagedAsync(int careerId, int page, int pageSize)
    {
        var query = _context.Students
            .Where(s => s.StudentCareers.Any(sc => sc.CareerId == careerId && sc.IsActive) && s.Activo);

        var totalCount = await query.CountAsync();
        
        var students = await query
            .Include(s => s.StudentCareers)
            .ThenInclude(sc => sc.Career)
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (students, totalCount);
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Students.AnyAsync(s => s.EstudianteId == id && s.Activo);
    }
}