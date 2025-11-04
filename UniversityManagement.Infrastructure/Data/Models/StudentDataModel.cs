using UniversityManagement.Infrastructure.Data.Common;

namespace UniversityManagement.Infrastructure.Data.Models;

/// <summary>
/// Modelo de datos para EF Core
/// </summary>
public class StudentDataModel : BaseDataModel
{
    public int EstudianteId { get; set; }
    
    public string Nombre { get; set; } = string.Empty;
    
    public string Apellido { get; set; } = string.Empty;
    
    public string Dni { get; set; } = string.Empty;
    
    public string Email { get; set; } = string.Empty;
    
    public string? Telefono { get; set; }
    
    public DateTime FechaNacimiento { get; set; }
    
    public string? Direccion { get; set; }

    // Propiedades de navegación para EF Core
    public virtual ICollection<StudentCareerDataModel> StudentCareers { get; set; } = new List<StudentCareerDataModel>();
}