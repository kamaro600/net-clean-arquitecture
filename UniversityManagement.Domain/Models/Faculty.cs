using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

public class Faculty : BaseEntity
{
    public int FacultyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Location { get; set; }
    public string? Dean { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Career> Careers { get; set; } = new List<Career>();
}