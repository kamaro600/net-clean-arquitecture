using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Career a DTOs de respuesta
/// </summary>
public static class CareerMapper
{
    /// <summary>
    /// Convierte un Career a CareerResponse
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
    /// Convierte un Career a CareerResponse incluyendo datos relacionados
    /// </summary>
    public static CareerResponse ToCareerData(this Career career, Faculty? faculty, IEnumerable<Student> students, IEnumerable<Professor> professors)
    {
        var studentList = students.ToList();
        var professorList = professors.ToList();
        
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
            FacultyName = faculty?.Name,
            FacultyDescription = faculty?.Description,
            TotalStudents = studentList.Count(s => s.IsActive),
            TotalProfessors = professorList.Count(p => p.Activo)
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