using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

public class Professor : BaseEntity
{
    public int ProfesorId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Especialidad { get; set; }
    public string? TituloAcademico { get; set; }

    // Propiedades de navegación
    public virtual ICollection<ProfessorCareer> ProfessorCareers { get; set; } = new List<ProfessorCareer>();
}