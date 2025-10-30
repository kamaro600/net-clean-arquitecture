namespace UniversityManagement.Application.DTOs.Queries;

/// <summary>
/// Consulta para obtener profesores paginados
/// </summary>
public class GetProfessorsQuery
{
    public string? SearchTerm { get; set; }
}