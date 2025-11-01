using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

// Entidad de relación muchos a muchos entre Professor y Career
public class ProfessorCareer
{
    public int ProfessorId { get; set; }
    public int CareerId { get; set; }
    public DateTime AssignmentDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Propiedades de navegación
    public virtual Professor Professor { get; set; } = null!;
    public virtual Career Career { get; set; } = null!;
}