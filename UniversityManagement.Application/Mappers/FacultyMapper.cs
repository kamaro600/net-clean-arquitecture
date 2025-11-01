using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Faculty a DTOs de respuesta
/// </summary>
public static class FacultyMapper
{
    /// <summary>
    /// Convierte un Faculty a FacultyResponse
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
            Careers = new List<FacultyCareerResponse>(), // Se debe obtener por separado
            TotalCareers = 0, // Se debe calcular por separado
            TotalStudents = 0, // Se debe calcular por separado
            TotalProfessors = 0 // Se debe calcular por separado
        };
    }

    /// <summary>
    /// Convierte un Faculty a FacultyResponse incluyendo las carreras relacionadas
    /// </summary>
    public static FacultyResponse ToFacultyData(this Faculty faculty, IEnumerable<Career> careers)
    {
        var careerList = careers.ToList();
        
        return new FacultyResponse
        {
            Id = faculty.FacultyId,
            Name = faculty.Name,
            Description = faculty.Description,
            Location = faculty.Location,
            Dean = faculty.Dean,
            IsActive = faculty.Activo,
            RegistrationDate = faculty.FechaRegistro,
            Careers = careerList.Select(c => c.ToFacultyCareerData()).ToList(),
            TotalCareers = careerList.Count(c => c.Activo),
            TotalStudents = 0, // Se puede calcular si es necesario
            TotalProfessors = 0 // Se puede calcular si es necesario
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