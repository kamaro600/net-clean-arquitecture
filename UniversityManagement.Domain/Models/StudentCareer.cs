using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

// Entidad de relación muchos a muchos entre Student y Career
public class StudentCareer
{
    public int EstudianteId { get; set; }
    public int CarreraId { get; set; }
    public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    // Propiedades de navegación
    public virtual Student Student { get; set; } = null!;
    public virtual Career Career { get; set; } = null!;
}