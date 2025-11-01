namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con profesores
/// </summary>
public class ProfessorResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Specialty { get; set; }
    public string? AcademicDegree { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegisterDate { get; set; }

    // Información de las carreras que enseña
    public List<ProfessorCareerResponse> Careers { get; set; } = new();

    // Estadísticas
    public int TotalCareers { get; set; }
    public int YearsOfExperience => DateTime.Now.Year - RegisterDate.Year;
}
