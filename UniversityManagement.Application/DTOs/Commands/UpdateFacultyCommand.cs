namespace UniversityManagement.Application.DTOs.Commands;

/// <summary>
/// Comando para actualizar una facultad existente
/// </summary>
public class UpdateFacultyCommand
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public bool? IsActive { get; set; }
}