using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

// Entidad de relación muchos a muchos entre Professor y Career
public class ProfessorCareer
{
    public int ProfesorId { get; set; }
    public int CarreraId { get; set; }
    public DateTime FechaAsignacion { get; set; } = DateTime.UtcNow;
    public bool Activo { get; set; } = true;

    // Propiedades de navegación
    public virtual Professor Professor { get; set; } = null!;
    public virtual Career Career { get; set; } = null!;
}