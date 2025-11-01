namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con facultades
/// </summary>
public class FacultyResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Dean { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegistrationDate { get; set; }

    // Información de las carreras
    public List<FacultyCareerResponse> Careers { get; set; } = new();

    // Estadísticas
    public int TotalCareers { get; set; }
    public int TotalStudents { get; set; }
    public int TotalProfessors { get; set; }
}
