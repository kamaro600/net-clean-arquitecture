namespace UniversityManagement.Application.DTOs.Queries;

/// <summary>
/// Consulta para obtener carreras paginadas
/// </summary>
public class GetCareersQuery
{
    public string? SearchTerm { get; set; }
}