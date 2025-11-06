using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Mappers;

namespace UniversityManagement.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio de facultades
/// </summary>
public class FacultyRepository : IFacultyRepository
{
    private readonly UniversityDbContext _context;

    public FacultyRepository(UniversityDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Faculty>> GetAllAsync()
    {
        var dataModels = await _context.FacultiesData
            .Where(f => f.Activo)
            .OrderBy(f => f.Name)
            .ToListAsync();

        return FacultyMapper.ToDomain(dataModels);
    }

    public async Task<Faculty?> GetByIdAsync(int id)
    {
        var dataModel = await _context.FacultiesData
            .FirstOrDefaultAsync(f => f.FacultyId == id);

        return dataModel != null ? FacultyMapper.ToDomain(dataModel) : null;
    }

    public async Task<Faculty?> GetByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(name));

        var dataModel = await _context.FacultiesData
            .FirstOrDefaultAsync(f => f.Name == name && f.Activo);

        return dataModel != null ? FacultyMapper.ToDomain(dataModel) : null;
    }

    public async Task<Faculty> CreateAsync(Faculty faculty)
    {
        if (faculty == null)
            throw new ArgumentNullException(nameof(faculty));

        var dataModel = FacultyMapper.ToDataModel(faculty);
        _context.FacultiesData.Add(dataModel);
        await _context.SaveChangesAsync();

        return FacultyMapper.ToDomain(dataModel);
    }

    public async Task<Faculty> UpdateAsync(Faculty faculty)
    {
        if (faculty == null)
            throw new ArgumentNullException(nameof(faculty));

        var existingDataModel = await _context.FacultiesData
            .FirstOrDefaultAsync(f => f.FacultyId == faculty.FacultyId);

        if (existingDataModel == null)
            throw new InvalidOperationException($"Facultad con ID {faculty.FacultyId} no encontrada");

        FacultyMapper.UpdateDataModelFromDomain(existingDataModel, faculty);
        await _context.SaveChangesAsync();

        return FacultyMapper.ToDomain(existingDataModel);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var dataModel = await _context.FacultiesData.FindAsync(id);
        if (dataModel == null)
            return false;

        // Soft delete
        dataModel.Activo = false;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(name));

        return await _context.FacultiesData.AnyAsync(f => f.Name == name && f.Activo);
    }

    public async Task<(List<Faculty> Faculties, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null)
    {
        var query = _context.FacultiesData
            .Where(f => f.Activo)
            .AsQueryable();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            var searchLower = searchTerm.ToLower();
            query = query.Where(f => 
                f.Name.ToLower().Contains(searchLower) ||
                (f.Description != null && f.Description.ToLower().Contains(searchLower)) ||
                (f.Location != null && f.Location.ToLower().Contains(searchLower)));
        }

        var totalCount = await query.CountAsync();
        
        var dataModels = await query
            .OrderBy(f => f.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var faculties = FacultyMapper.ToDomain(dataModels).ToList();
        return (faculties, totalCount);
    }
}