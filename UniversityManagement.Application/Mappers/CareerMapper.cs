using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Career a DTOs de respuesta
/// </summary>
public static class CareerMapper
{

    /// <summary>
    /// Convierte un CareerDomain a CareerResponse
    /// </summary>
    public static CareerResponse ToCareerData(this Career career)
    {
        return new CareerResponse
        {
            Id = career.CareerId,
            Name = career.Name,
            Description = career.Description,
            SemesterDuration = career.SemesterDuration,
            AwardedTitle = career.AwardedTitle,
            IsActive = career.Activo,
            RegistrationDate = career.FechaRegistro,
            FacultyId = career.FacultyId,
            FacultyName = null, // Se debe obtener por separado usando repository
            FacultyDescription = null, // Se debe obtener por separado usando repository
            TotalStudents = 0, // Se debe calcular por separado usando repository
            TotalProfessors = 0 // Se debe calcular por separado usando repository
        };
    }

    /// <summary>
    /// Convierte una lista de CareerDomain a lista de CareerResponse
    /// </summary>
    public static List<CareerResponse> ToCareerDataList(this IEnumerable<Career> careers)
    {
        return careers.Select(c => c.ToCareerData()).ToList();
    }
}