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
            Id = career.CarreraId,
            Nombre = career.Nombre,
            Descripcion = career.Descripcion,
            DuracionSemestres = career.DuracionSemestres,
            TituloOtorgado = career.TituloOtorgado,
            Activo = career.Activo,
            FechaRegistro = career.FechaRegistro,
            FacultadId = career.FacultadId,
            FacultadNombre = career.Faculty?.Nombre,
            FacultadDescripcion = career.Faculty?.Descripcion,
            TotalEstudiantes = career.StudentCareers?.Count(sc => sc.Activo) ?? 0,
            TotalProfesores = career.ProfessorCareers?.Count(pc => pc.Activo) ?? 0
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