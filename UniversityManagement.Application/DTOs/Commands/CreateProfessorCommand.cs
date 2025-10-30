namespace UniversityManagement.Application.DTOs.Commands;

/// <summary>
/// Comando para crear un nuevo profesor
/// </summary>
public class CreateProfessorCommand
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Specialty { get; set; }
}