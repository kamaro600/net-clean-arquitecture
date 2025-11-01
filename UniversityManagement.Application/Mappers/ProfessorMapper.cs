using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Professor a DTOs de respuesta
/// </summary>
public static class ProfessorMapper
{
    /// <summary>
    /// Convierte un Professor a ProfessorResponse
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
    /// Convierte un Professor a ProfessorResponse incluyendo las carreras relacionadas
    /// </summary>
    public static ProfessorResponse ToProfessorData(this Professor professor, IEnumerable<Career> careers)
    {
        var careerList = careers.ToList();
        
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
            Careers = careerList.Select(c => c.ToProfessorCareerData()).ToList(),
            TotalCareers = careerList.Count(c => c.Activo)
        };
    }

    /// <summary>
    /// Convierte un Career a ProfessorCareerResponse
    /// </summary>
    public static ProfessorCareerResponse ToProfessorCareerData(this Career career)
    {
        return new ProfessorCareerResponse
        {
            CareerId = career.CareerId,
            CareerName = career.Name,
            CareerDescription = career.Description,
            FacultyName = null, // Se debe obtener por separado si es necesario
            AssignmentDate = DateTime.Now, // Se debe obtener del ProfessorCareer si está disponible
            IsActive = career.Activo
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