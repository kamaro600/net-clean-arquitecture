using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

// Entidad de relación muchos a muchos entre Student y Career
public class StudentCareer
{
    public int StudentId { get; set; }
    public int CareerId { get; set; }
    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

    // Propiedades de navegación
    public virtual Student Student { get; set; } = null!;
    public virtual Career Career { get; set; } = null!;
}