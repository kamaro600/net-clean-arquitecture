using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Mappers;

namespace UniversityManagement.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de carreras
/// </summary>
public class CareerRepository : ICareerRepository
{
    private readonly UniversityDbContext _context;

    public CareerRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Career>> GetAllAsync()
    {
        var dataModels = await _context.CareersData
            .Where(c => c.Activo)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return CareerMapper.ToDomain(dataModels);
    }

    public async Task<Career?> GetByIdAsync(int id)
    {
        var dataModel = await _context.CareersData
            .FirstOrDefaultAsync(c => c.CareerId == id);

        return dataModel != null ? CareerMapper.ToDomain(dataModel) : null;
    }

    public async Task<Career?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(name));

        var dataModel = await _context.CareersData
            .FirstOrDefaultAsync(c => c.Name == name && c.Activo);

        return dataModel != null ? CareerMapper.ToDomain(dataModel) : null;
    }

    public async Task<IEnumerable<Career>> GetByFacultyIdAsync(int facultyId)
    {
        var dataModels = await _context.CareersData
            .Where(c => c.FacultyId == facultyId && c.Activo)
            .OrderBy(c => c.Name)
            .ToListAsync();

        return CareerMapper.ToDomain(dataModels);
    }

    public async Task<Career> CreateAsync(Career career)
    {
        if (career == null)
            throw new ArgumentNullException(nameof(career));

        var dataModel = CareerMapper.ToDataModel(career);
        _context.CareersData.Add(dataModel);
        await _context.SaveChangesAsync();

        return CareerMapper.ToDomain(dataModel);
    }

    public async Task<Career> UpdateAsync(Career career)
    {
        if (career == null)
            throw new ArgumentNullException(nameof(career));

        var existingDataModel = await _context.CareersData
            .FirstOrDefaultAsync(c => c.CareerId == career.CareerId);

        if (existingDataModel == null)
            throw new InvalidOperationException($"Carrera con ID {career.CareerId} no encontrada");

        CareerMapper.UpdateDataModelFromDomain(existingDataModel, career);
        await _context.SaveChangesAsync();

        return CareerMapper.ToDomain(existingDataModel);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dataModel = await _context.CareersData.FindAsync(id);
        if (dataModel == null)
            return false;

        // Soft delete
        dataModel.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameInFacultyAsync(string name, int facultyId)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(name));

        return await _context.CareersData.AnyAsync(c => 
            c.Name == name && c.FacultyId == facultyId && c.Activo);
    }

    public async Task<(List<Career> Careers, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.CareersData
            .Where(c => c.Activo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(c => 
                c.Name.ToLower().Contains(searchLower) ||
                (c.Description != null && c.Description.ToLower().Contains(searchLower)) ||
                (c.AwardedTitle != null && c.AwardedTitle.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync();
        
        var dataModels = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var careers = CareerMapper.ToDomain(dataModels).ToList();
        return (careers, totalCount);
    }
}