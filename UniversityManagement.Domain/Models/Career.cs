using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

public class Career : BaseEntity
{
    public int CareerId { get; set; }
    public int FacultyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int SemesterDuration { get; set; }
    public string? AwardedTitle { get; set; }

    // Propiedades de navegación
    public virtual Faculty Faculty { get; set; } = null!;
    public virtual ICollection<StudentCareer> StudentCareers { get; set; } = new List<StudentCareer>();
    public virtual ICollection<ProfessorCareer> ProfessorCareers { get; set; } = new List<ProfessorCareer>();

}