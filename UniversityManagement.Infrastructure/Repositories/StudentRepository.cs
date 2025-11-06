using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Models.ValueObjects;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Data.Models;
using UniversityManagement.Infrastructure.Mappers;

namespace UniversityManagement.Infrastructure.Repositories;

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
        var dataModels = await _context.StudentsData
            .Where(s => s.Activo)
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .ToListAsync();

        return StudentMapper.ToDomain(dataModels);
    }

    public async Task<Student?> GetByIdAsync(int id)
    {
        var dataModel = await _context.StudentsData
            // Temporalmente comentamos Include para evitar conflictos
            // .Include(s => s.StudentCareers)
            // .ThenInclude(sc => sc.Career)
            .FirstOrDefaultAsync(s => s.EstudianteId == id);

        return dataModel != null ? StudentMapper.ToDomain(dataModel) : null;
    }

    public async Task<Student?> GetByDniAsync(Dni dni)
    {
        if (dni == null)
            throw new ArgumentNullException(nameof(dni));

        var dataModel = await _context.StudentsData
            .FirstOrDefaultAsync(s => s.Dni == dni.Value);

        return dataModel != null ? StudentMapper.ToDomain(dataModel) : null;
    }

    public async Task<Student?> GetByEmailAsync(Email email)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        var dataModel = await _context.StudentsData
            .FirstOrDefaultAsync(s => s.Email == email.Value);

        return dataModel != null ? StudentMapper.ToDomain(dataModel) : null;
    }

    public async Task<Student> CreateAsync(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        var dataModel = StudentMapper.ToDataModel(student);
        _context.StudentsData.Add(dataModel);
        await _context.SaveChangesAsync();

        // Retornar la entidad de dominio con el ID generado
        return StudentMapper.ToDomain(dataModel);
    }

    public async Task<Student> UpdateAsync(Student student)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));

        var existingDataModel = await _context.StudentsData
            .FirstOrDefaultAsync(s => s.EstudianteId == student.Id);

        if (existingDataModel == null)
            throw new InvalidOperationException($"Estudiante con ID {student.Id} no encontrado");

        // Actualizar el modelo existente preservando el tracking de EF Core
        StudentMapper.UpdateDataModelFromDomain(existingDataModel, student);
        await _context.SaveChangesAsync();

        return StudentMapper.ToDomain(existingDataModel);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dataModel = await _context.StudentsData.FindAsync(id);
        if (dataModel == null)
            return false;

        // Soft delete
        dataModel.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByDniAsync(Dni dni)
    {
        if (dni == null)
            throw new ArgumentNullException(nameof(dni));

        return await _context.StudentsData.AnyAsync(s => s.Dni == dni.Value && s.Activo);
    }

    public async Task<bool> ExistsByEmailAsync(Email email)
    {
        if (email == null)
            throw new ArgumentNullException(nameof(email));

        return await _context.StudentsData.AnyAsync(s => s.Email == email.Value && s.Activo);
    }

    public async Task<(List<Student> Students, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.StudentsData
            .Where(s => s.Activo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(s => 
                s.Nombre.ToLower().Contains(searchLower) ||
                s.Apellido.ToLower().Contains(searchLower) ||
                s.Dni.Contains(searchLower) ||
                s.Email.ToLower().Contains(searchLower));
        }

        var totalCount = await query.CountAsync();
        
        var dataModels = await query
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var students = StudentMapper.ToDomain(dataModels).ToList();
        return (students, totalCount);
    }

    public async Task<IEnumerable<Student>> GetStudentsByCareerId(int careerId)
    {
        // Temporalmente simplificamos esta consulta hasta que las relaciones estén completas
        var dataModels = await _context.StudentsData
            .Where(s => s.Activo)
            // .Where(s => s.Activo && s.StudentCareers.Any(sc => sc.CarreraId == careerId && sc.Activo))
            .OrderBy(s => s.Apellido)
            .ThenBy(s => s.Nombre)
            .ToListAsync();

        return StudentMapper.ToDomain(dataModels);
    }

    public async Task<IEnumerable<Career>> GetCareersByStudentIdAsync(int studentId)
    {
        var careerDataModels = await _context.StudentCareersData
            .Where(sc => sc.StudentId == studentId && sc.IsActive)
            .Include(sc => sc.Career)
            .Select(sc => sc.Career)
            .Where(c => c != null && c.Activo)
            .ToListAsync();

        return CareerMapper.ToDomain(careerDataModels!);
    }
}