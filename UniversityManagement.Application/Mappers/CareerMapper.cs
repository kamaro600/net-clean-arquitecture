using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Career a DTOs de respuesta
/// </summary>
public static class CareerMapper
{
    /// <summary>
    /// Convierte un Career de dominio a CareerResponse
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
            FacultyName = career.Faculty?.Name,
            FacultyDescription = career.Faculty?.Description,
            TotalStudents = career.StudentCareers?.Count(sc => sc.IsActive) ?? 0,
            TotalProfessors = career.ProfessorCareers?.Count(pc => pc.IsActive) ?? 0
        };
    }

    /// <summary>
    /// Convierte una lista de Career a lista de CareerResponse
    /// </summary>
    public static List<CareerResponse> ToCareerDataList(this IEnumerable<Career> careers)
    {
        return careers.Select(c => c.ToCareerData()).ToList();
    }

}