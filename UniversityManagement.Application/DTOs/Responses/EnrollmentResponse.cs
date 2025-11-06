namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones de matrícula
/// </summary>
public class EnrollmentResponse
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentDni { get; set; } = string.Empty;
    public int CareerId { get; set; }
    public string CareerName { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}