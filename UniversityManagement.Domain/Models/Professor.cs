using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

public class Professor : BaseEntity
{
    public int ProfessorId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Specialty { get; set; }
    public string? AcademicDegree { get; set; }

    // Propiedades de navegación
    public virtual ICollection<ProfessorCareer> ProfessorCareers { get; set; } = new List<ProfessorCareer>();
}