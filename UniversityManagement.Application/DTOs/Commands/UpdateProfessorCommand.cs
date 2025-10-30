namespace UniversityManagement.Application.DTOs.Commands;

/// <summary>
/// Comando para actualizar un profesor existente
/// </summary>
public class UpdateProfessorCommand
{
    public int Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Dni { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Specialty { get; set; }
    public bool? IsActive { get; set; }
}