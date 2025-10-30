namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con facultades
/// </summary>
public class FacultyResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
    public string? Decano { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }

    // Información de las carreras
    public List<FacultyCareerResponse> Carreras { get; set; } = new();

    // Estadísticas
    public int TotalCarreras { get; set; }
    public int TotalEstudiantes { get; set; }
    public int TotalProfesores { get; set; }
}
