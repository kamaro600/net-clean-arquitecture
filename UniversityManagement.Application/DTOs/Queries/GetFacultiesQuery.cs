namespace UniversityManagement.Application.DTOs.Queries;

/// <summary>
/// Consulta para obtener facultades paginadas
/// </summary>
public class GetFacultiesQuery
{
    public string? SearchTerm { get; set; }
}