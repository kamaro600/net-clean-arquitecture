using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Professor a DTOs de respuesta
/// </summary>
public static class ProfessorMapper
{
    /// <summary>
    /// Convierte un Professor de dominio a ProfessorResponse
    /// </summary>
    public static ProfessorResponse ToProfessorData(this Professor professor)
    {
        return new ProfessorResponse
        {
            Id = professor.ProfessorId,
            FirstName = professor.FirstName,
            LastName = professor.LastName,
            Dni = professor.Dni,
            Email = professor.Email,
            Phone = professor.Phone,
            Specialty = professor.Specialty,
            AcademicDegree = professor.AcademicDegree,
            IsActive = professor.Activo,
            RegisterDate = professor.FechaRegistro,
            Careers = professor.ProfessorCareers?.Select(pc => pc.ToProfessorCareerData()).ToList() ?? new List<ProfessorCareerResponse>(),
            TotalCareers = professor.ProfessorCareers?.Count(pc => pc.IsActive) ?? 0
        };
    }

    /// <summary>
    /// Convierte un ProfessorCareer a ProfessorCareerResponse
    /// </summary>
    public static ProfessorCareerResponse ToProfessorCareerData(this ProfessorCareer professorCareer)
    {
        return new ProfessorCareerResponse
        {
            CareerId = professorCareer.CareerId,
            CareerName = professorCareer.Career?.Name ?? string.Empty,
            CareerDescription = professorCareer.Career?.Description,
            FacultyName = professorCareer.Career?.Faculty?.Name,
            AssignmentDate = professorCareer.AssignmentDate,
            IsActive = professorCareer.IsActive
        };
    }

    /// <summary>
    /// Convierte una lista de Professor a lista de ProfessorResponse
    /// </summary>
    public static List<ProfessorResponse> ToProfessorDataList(this IEnumerable<Professor> professors)
    {
        return professors.Select(p => p.ToProfessorData()).ToList();
    }
  
}