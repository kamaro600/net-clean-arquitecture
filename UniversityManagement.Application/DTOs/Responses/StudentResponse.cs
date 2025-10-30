using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con estudiantes
/// </summary>
public class StudentResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }
    public List<StudentCareerResponse> Carreras { get; set; } = new();
}