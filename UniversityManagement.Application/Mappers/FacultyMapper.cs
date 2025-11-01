using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Faculty a DTOs de respuesta
/// </summary>
public static class FacultyMapper
{

    /// <summary>
    /// Convierte un FacultyDomain a FacultyResponse
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
    /// Convierte una lista de FacultyDomain a lista de FacultyResponse
    /// </summary>
    public static List<FacultyResponse> ToFacultyDataList(this IEnumerable<Faculty> faculties)
    {
        return faculties.Select(f => f.ToFacultyData()).ToList();
    }
}