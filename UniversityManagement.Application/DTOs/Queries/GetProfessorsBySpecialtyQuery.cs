namespace UniversityManagement.Application.DTOs.Queries;

/// <summary>
/// Consulta para obtener profesores por especialidad
/// </summary>
public class GetProfessorsBySpecialtyQuery
{
    public string Specialty { get; set; } = string.Empty;
}