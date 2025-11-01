using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Faculty a DTOs de respuesta
/// </summary>
public static class FacultyMapper
{
    /// <summary>
    /// Convierte un Faculty de dominio a FacultyResponse
    /// </summary>
    public static FacultyResponse ToFacultyData(this Faculty faculty)
    {
        return new FacultyResponse
        {
            Id = faculty.FacultyId,
            Name = faculty.Name,
            Description = faculty.Description,
            Location = faculty.Location,
            Dean = faculty.Dean,
            IsActive = faculty.Activo,
            RegistrationDate = faculty.FechaRegistro,
            Careers = faculty.Careers?.Select(c => c.ToFacultyCareerData()).ToList() ?? new List<FacultyCareerResponse>(),
            TotalCareers = faculty.Careers?.Count(c => c.Activo) ?? 0,
            TotalStudents = faculty.Careers?.SelectMany(c => c.StudentCareers).Count(sc => sc.IsActive) ?? 0,
            TotalProfessors = faculty.Careers?.SelectMany(c => c.ProfessorCareers).Count(pc => pc.IsActive) ?? 0
        };
    }

    /// <summary>
    /// Convierte un Career a FacultyCareerResponse
    /// </summary>
    public static FacultyCareerResponse ToFacultyCareerData(this Career career)
    {
        return new FacultyCareerResponse
        {
            Id = career.CareerId,
            Name = career.Name,
            Description = career.Description,
            SemesterDuration = career.SemesterDuration,
            IsActive = career.Activo
        };
    }

    /// <summary>
    /// Convierte una lista de Faculty a lista de FacultyResponse
    /// </summary>
    public static List<FacultyResponse> ToFacultyDataList(this IEnumerable<Faculty> faculties)
    {
        return faculties.Select(f => f.ToFacultyData()).ToList();
    }
  
}