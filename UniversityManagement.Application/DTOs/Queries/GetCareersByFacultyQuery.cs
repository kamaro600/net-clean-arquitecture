namespace UniversityManagement.Application.DTOs.Queries;

/// <summary>
/// Consulta para obtener carreras de una facultad específica
/// </summary>
public class GetCareersByFacultyQuery
{
    public int FacultyId { get; set; }
}