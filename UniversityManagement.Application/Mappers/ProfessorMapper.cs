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
            Id = professor.ProfesorId,
            Nombre = professor.Nombre,
            Apellido = professor.Apellido,
            Dni = professor.Dni,
            Email = professor.Email,
            Telefono = professor.Telefono,
            Especialidad = professor.Especialidad,
            TituloAcademico = professor.TituloAcademico,
            Activo = professor.Activo,
            FechaRegistro = professor.FechaRegistro,
            Carreras = professor.ProfessorCareers?.Select(pc => pc.ToProfessorCareerData()).ToList() ?? new List<ProfessorCareerResponse>(),
            TotalCarreras = professor.ProfessorCareers?.Count(pc => pc.Activo) ?? 0
        };
    }

    /// <summary>
    /// Convierte un ProfessorCareer a ProfessorCareerResponse
    /// </summary>
    public static ProfessorCareerResponse ToProfessorCareerData(this ProfessorCareer professorCareer)
    {
        return new ProfessorCareerResponse
        {
            CarreraId = professorCareer.CarreraId,
            NombreCarrera = professorCareer.Career?.Nombre ?? string.Empty,
            DescripcionCarrera = professorCareer.Career?.Descripcion,
            FacultadNombre = professorCareer.Career?.Faculty?.Nombre,
            FechaAsignacion = professorCareer.FechaAsignacion,
            Activo = professorCareer.Activo
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