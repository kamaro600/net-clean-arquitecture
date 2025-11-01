using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con estudiantes
/// </summary>
public class StudentResponse
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName => $"{FirstName} {LastName}";
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public DateTime BirthDate { get; set; }
    public string? Address { get; set; }
    public bool IsActive { get; set; }
    public DateTime RegisterDate { get; set; }
    public List<StudentCareerResponse> Careers { get; set; } = new();
}