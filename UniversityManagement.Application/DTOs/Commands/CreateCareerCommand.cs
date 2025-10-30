namespace UniversityManagement.Application.DTOs.Commands;

/// <summary>
/// Comando para crear una nueva carrera
/// </summary>
public class CreateCareerCommand
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int FacultyId { get; set; }
}