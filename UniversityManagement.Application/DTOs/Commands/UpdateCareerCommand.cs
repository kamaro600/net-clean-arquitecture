namespace UniversityManagement.Application.DTOs.Commands;

/// <summary>
/// Comando para actualizar una carrera existente
/// </summary>
public class UpdateCareerCommand
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? FacultyId { get; set; }
    public bool? IsActive { get; set; }
}