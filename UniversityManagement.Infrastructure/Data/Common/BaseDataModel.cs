namespace UniversityManagement.Infrastructure.Data.Common;

/// <summary>
/// Clase base para modelos de datos de EF Core
/// Contiene propiedades comunes de auditoría y estado
/// </summary>
public abstract class BaseDataModel
{
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    
    public bool Activo { get; set; } = true;
}