namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response específico para operaciones con carreras
/// </summary>
public class CareerResponse
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int DuracionSemestres { get; set; }
    public string? TituloOtorgado { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaRegistro { get; set; }

    // Información de la facultad
    public int FacultadId { get; set; }
    public string? FacultadNombre { get; set; }
    public string? FacultadDescripcion { get; set; }

    // Estadísticas
    public int TotalEstudiantes { get; set; }
    public int TotalProfesores { get; set; }
}