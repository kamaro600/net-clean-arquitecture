namespace UniversityManagement.Domain.Common;

/// <summary>
/// Clase base para todas las entidades del dominio
/// Contiene propiedades comunes como auditoría
/// </summary>
public abstract class BaseEntity
{
    public DateTime FechaRegistro { get; set; } = DateTime.Now;
    public bool Activo { get; set; } = true;
}