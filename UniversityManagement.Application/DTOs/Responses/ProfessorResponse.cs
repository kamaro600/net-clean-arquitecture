namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con profesores
/// </summary>
public class ProfessorResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string NombreCompleto => $"{Nombre} {Apellido}";
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Especialidad { get; set; }
    public string? TituloAcademico { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }

    // Información de las carreras que enseña
    public List<ProfessorCareerResponse> Carreras { get; set; } = new();

    // Estadísticas
    public int TotalCarreras { get; set; }
    public int AniosExperiencia => DateTime.Now.Year - FechaRegistro.Year;
}
