using UniversityManagement.Domain.Common;

namespace UniversityManagement.Domain.Models;

public class Career : BaseEntity
{
    public int CarreraId { get; set; }
    public int FacultadId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int DuracionSemestres { get; set; }
    public string? TituloOtorgado { get; set; }

    // Propiedades de navegación
    public virtual Faculty Faculty { get; set; } = null!;
    public virtual ICollection<StudentCareer> StudentCareers { get; set; } = new List<StudentCareer>();
    public virtual ICollection<ProfessorCareer> ProfessorCareers { get; set; } = new List<ProfessorCareer>();

}