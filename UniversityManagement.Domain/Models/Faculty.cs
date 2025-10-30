using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

public class Faculty : BaseEntity
{
    public int FacultadId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public string? Ubicacion { get; set; }
    public string? Decano { get; set; }

    // Propiedades de navegación
    public virtual ICollection<Career> Careers { get; set; } = new List<Career>();
}