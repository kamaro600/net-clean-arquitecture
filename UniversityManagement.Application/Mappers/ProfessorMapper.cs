using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Professor a DTOs de respuesta
/// </summary>
public static class ProfessorMapper
{

    /// <summary>
    /// Convierte un ProfessorDomain a ProfessorResponse
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
            Careers = new List<ProfessorCareerResponse>(), // Se debe obtener por separado
            TotalCareers = 0 // Se debe calcular por separado
        };
    }

    /// <summary>
    /// Convierte una lista de ProfessorDomain a lista de ProfessorResponse
    /// </summary>
    public static List<ProfessorResponse> ToProfessorDataList(this IEnumerable<Professor> professors)
    {
        return professors.Select(p => p.ToProfessorData()).ToList();
    }
}