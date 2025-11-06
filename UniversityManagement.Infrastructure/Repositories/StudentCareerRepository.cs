using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Data.Models;
using UniversityManagement.Infrastructure.Mappers;

namespace UniversityManagement.Infrastructure.Repositories;

/// <summary>
/// Implementación del repositorio para matriculación (relación estudiante-carrera)
/// </summary>
public class StudentCareerRepository : IStudentCareerRepository
{
    private readonly UniversityDbContext _context;
    private readonly StudentCareerMapper _mapper;

    public StudentCareerRepository(UniversityDbContext context, StudentCareerMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<StudentCareer?> GetEnrollmentAsync(int studentId, int careerId)
    {
        var dataModel = await _context.StudentCareersData
            .Include(sc => sc.Student)
            .Include(sc => sc.Career)
                .ThenInclude(c => c.Faculty)
            .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CareerId == careerId);

        return dataModel != null ? _mapper.DataModelToDomain(dataModel) : null;
    }

    public async Task<IEnumerable<StudentCareer>> GetStudentEnrollmentsAsync(int studentId)
    {
        var dataModels = await _context.StudentCareersData
            .Include(sc => sc.Student)
            .Include(sc => sc.Career)
                .ThenInclude(c => c.Faculty)
            .Where(sc => sc.StudentId == studentId)
            .ToListAsync();

        return dataModels.Select(_mapper.DataModelToDomain);
    }

    public async Task<IEnumerable<StudentCareer>> GetCareerEnrollmentsAsync(int careerId)
    {
        var dataModels = await _context.StudentCareersData
            .Include(sc => sc.Student)
            .Include(sc => sc.Career)
                .ThenInclude(c => c.Faculty)
            .Where(sc => sc.CareerId == careerId)
            .ToListAsync();

        return dataModels.Select(_mapper.DataModelToDomain);
    }

    public async Task<StudentCareer> AddEnrollmentAsync(StudentCareer enrollment)
    {
        var dataModel = _mapper.DomainToDataModel(enrollment);
        _context.StudentCareersData.Add(dataModel);
        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(dataModel)
            .Reference(sc => sc.Student)
            .LoadAsync();
        
        await _context.Entry(dataModel)
            .Reference(sc => sc.Career)
            .LoadAsync();
        
        await _context.Entry(dataModel.Career!)
            .Reference(c => c.Faculty)
            .LoadAsync();

        return _mapper.DataModelToDomain(dataModel);
    }

    public async Task<StudentCareer> UpdateEnrollmentAsync(StudentCareer enrollment)
    {
        var existingDataModel = await _context.StudentCareersData
            .FirstOrDefaultAsync(sc => sc.StudentId == enrollment.StudentId && sc.CareerId == enrollment.CareerId);

        if (existingDataModel == null)
        {
            throw new InvalidOperationException($"Enrollment not found for Student {enrollment.StudentId} and Career {enrollment.CareerId}");
        }

        // Update properties
        existingDataModel.EnrollmentDate = enrollment.EnrollmentDate;
        existingDataModel.IsActive = enrollment.IsActive;

        await _context.SaveChangesAsync();

        // Reload with related data
        await _context.Entry(existingDataModel)
            .Reference(sc => sc.Student)
            .LoadAsync();
        
        await _context.Entry(existingDataModel)
            .Reference(sc => sc.Career)
            .LoadAsync();
        
        await _context.Entry(existingDataModel.Career!)
            .Reference(c => c.Faculty)
            .LoadAsync();

        return _mapper.DataModelToDomain(existingDataModel);
    }

    public async Task<bool> DeleteEnrollmentAsync(int studentId, int careerId)
    {
        var dataModel = await _context.StudentCareersData
            .FirstOrDefaultAsync(sc => sc.StudentId == studentId && sc.CareerId == careerId);

        if (dataModel == null)
        {
            return false;
        }

        _context.StudentCareersData.Remove(dataModel);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsActiveEnrollmentAsync(int studentId, int careerId)
    {
        return await _context.StudentCareersData
            .AnyAsync(sc => sc.StudentId == studentId && sc.CareerId == careerId && sc.IsActive);
    }

    public async Task<int> GetActiveEnrollmentCountAsync(int studentId)
    {
        return await _context.StudentCareersData
            .CountAsync(sc => sc.StudentId == studentId && sc.IsActive);
    }
}