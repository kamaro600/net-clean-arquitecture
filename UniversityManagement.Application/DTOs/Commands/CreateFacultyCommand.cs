namespace UniversityManagement.Application.DTOs.Commands;

/// <summary>
/// Comando para crear una nueva facultad
/// </summary>
public class CreateFacultyCommand
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}