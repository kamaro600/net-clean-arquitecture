namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con carreras
/// </summary>
public class CareerResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SemesterDuration { get; set; }
    public string? AwardedTitle { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegistrationDate { get; set; }

    // Información de la facultad
    public int FacultyId { get; set; }
    public string? FacultyName { get; set; }
    public string? FacultyDescription { get; set; }

    // Estadísticas
    public int TotalStudents { get; set; }
    public int TotalProfessors { get; set; }
}