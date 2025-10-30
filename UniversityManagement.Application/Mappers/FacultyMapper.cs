using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Faculty a DTOs de respuesta
/// </summary>
public static class FacultyMapper
{
    /// <summary>
    /// Convierte un Faculty de dominio a FacultyData
    /// </summary>
    public static FacultyResponse ToFacultyData(this Faculty faculty)
    {
        return new FacultyResponse
        {
            Id = faculty.FacultadId,
            Nombre = faculty.Nombre,
            Descripcion = faculty.Descripcion,
            Ubicacion = faculty.Ubicacion,
            Decano = faculty.Decano,
            Activo = faculty.Activo,
            FechaRegistro = faculty.FechaRegistro,
            Carreras = faculty.Careers?.Select(c => c.ToFacultyCareerData()).ToList() ?? new List<FacultyCareerResponse>(),
            TotalCarreras = faculty.Careers?.Count(c => c.Activo) ?? 0,
            TotalEstudiantes = faculty.Careers?.SelectMany(c => c.StudentCareers).Count(sc => sc.Activo) ?? 0,
            TotalProfesores = faculty.Careers?.SelectMany(c => c.ProfessorCareers).Count(pc => pc.Activo) ?? 0
        };
    }

    /// <summary>
    /// Convierte un Career a FacultyCareerData (datos básicos para la facultad)
    /// </summary>
    public static FacultyCareerResponse ToFacultyCareerData(this Career career)
    {
        return new FacultyCareerResponse
        {
            Id = career.CarreraId,
            Nombre = career.Nombre,
            Descripcion = career.Descripcion,
            DuracionSemestres = career.DuracionSemestres,
            Activo = career.Activo
        };
    }

    /// <summary>
    /// Convierte una lista de Faculty a lista de FacultyData
    /// </summary>
    public static List<FacultyResponse> ToFacultyDataList(this IEnumerable<Faculty> faculties)
    {
        return faculties.Select(f => f.ToFacultyData()).ToList();
    }
  
}