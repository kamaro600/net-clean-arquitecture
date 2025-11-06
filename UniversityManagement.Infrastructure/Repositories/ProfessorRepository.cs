using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Mappers;

namespace UniversityManagement.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de profesores
/// </summary>
public class ProfessorRepository : IProfessorRepository
{
    private readonly UniversityDbContext _context;

    public ProfessorRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Professor>> GetAllAsync()
    {
        var dataModels = await _context.ProfessorsData
            .Where(p => p.Activo)
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();

        return ProfessorMapper.ToDomain(dataModels);
    }

    public async Task<Professor?> GetByIdAsync(int id)
    {
        var dataModel = await _context.ProfessorsData
            .FirstOrDefaultAsync(p => p.ProfessorId == id);

        return dataModel != null ? ProfessorMapper.ToDomain(dataModel) : null;
    }

    public async Task<Professor?> GetByDniAsync(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            throw new ArgumentException("El DNI no puede estar vacío", nameof(dni));

        var dataModel = await _context.ProfessorsData
            .FirstOrDefaultAsync(p => p.Dni == dni && p.Activo);

        return dataModel != null ? ProfessorMapper.ToDomain(dataModel) : null;
    }

    public async Task<Professor?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío", nameof(email));

        var dataModel = await _context.ProfessorsData
            .FirstOrDefaultAsync(p => p.Email == email && p.Activo);

        return dataModel != null ? ProfessorMapper.ToDomain(dataModel) : null;
    }

    public async Task<Professor> CreateAsync(Professor professor)
    {
        if (professor == null)
            throw new ArgumentNullException(nameof(professor));

        var dataModel = ProfessorMapper.ToDataModel(professor);
        _context.ProfessorsData.Add(dataModel);
        await _context.SaveChangesAsync();

        return ProfessorMapper.ToDomain(dataModel);
    }

    public async Task<Professor> UpdateAsync(Professor professor)
    {
        if (professor == null)
            throw new ArgumentNullException(nameof(professor));

        var existingDataModel = await _context.ProfessorsData
            .FirstOrDefaultAsync(p => p.ProfessorId == professor.ProfessorId);

        if (existingDataModel == null)
            throw new InvalidOperationException($"Profesor con ID {professor.ProfessorId} no encontrado");

        ProfessorMapper.UpdateDataModelFromDomain(existingDataModel, professor);
        await _context.SaveChangesAsync();

        return ProfessorMapper.ToDomain(existingDataModel);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dataModel = await _context.ProfessorsData.FindAsync(id);
        if (dataModel == null)
            return false;

        // Soft delete
        dataModel.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByDniAsync(string dni)
    {
        if (string.IsNullOrWhiteSpace(dni))
            throw new ArgumentException("El DNI no puede estar vacío", nameof(dni));

        return await _context.ProfessorsData.AnyAsync(p => p.Dni == dni && p.Activo);
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío", nameof(email));

        return await _context.ProfessorsData.AnyAsync(p => p.Email == email && p.Activo);
    }

    public async Task<IEnumerable<Professor>> GetProfessorsByCareerId(int careerId)
    {
        // Temporalmente simplificamos esta consulta hasta que las relaciones estén completas
        var dataModels = await _context.ProfessorsData
            .Where(p => p.Activo)
            // .Where(p => p.Activo && p.ProfessorCareers.Any(pc => pc.CareerId == careerId && pc.IsActive))
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .ToListAsync();

        return ProfessorMapper.ToDomain(dataModels);
    }

    public async Task<(List<Professor> Professors, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.ProfessorsData
            .Where(p => p.Activo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(p => 
                p.FirstName.ToLower().Contains(searchLower) ||
                p.LastName.ToLower().Contains(searchLower) ||
                p.Dni.Contains(searchLower) ||
                p.Email.ToLower().Contains(searchLower) ||
                (p.Specialty != null && p.Specialty.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync();
        
        var dataModels = await query
            .OrderBy(p => p.LastName)
            .ThenBy(p => p.FirstName)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var professors = ProfessorMapper.ToDomain(dataModels).ToList();
        return (professors, totalCount);
    }

    public async Task<IEnumerable<Career>> GetCareersByProfessorIdAsync(int professorId)
    {
        var careerDataModels = await _context.ProfessorCareersData
            .Where(pc => pc.ProfessorId == professorId && pc.IsActive)
            .Include(pc => pc.Career)
            .Select(pc => pc.Career)
            .Where(c => c != null && c.Activo)
            .ToListAsync();

        return CareerMapper.ToDomain(careerDataModels!);
    }
}