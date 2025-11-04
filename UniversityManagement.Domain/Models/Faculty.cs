namespace UniversityManagement.Domain.Models;

/// <summary>
/// Entidad de dominio pura para Facultad - Solo propiedades de dominio, sin dependencias de persistencia
/// </summary>
public class Faculty
{
    public int FacultyId { get; }
    public string Name { get; }
    public string? Description { get; }
    public string? Location { get; }
    public string? Dean { get; }
    public DateTime FechaRegistro { get; }
    public bool Activo { get; }

    /// <summary>
    /// Constructor principal para crear una facultad de dominio
    /// </summary>
    public Faculty(
        int facultyId,
        string name,
        string? description = null,
        string? location = null,
        string? dean = null,
        DateTime? fechaRegistro = null,
        bool activo = true)
    {
        if (facultyId < 0)
            throw new ArgumentException("El FacultyId debe ser mayor o igual a 0", nameof(facultyId));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la facultad no puede estar vacío", nameof(name));
        
        FacultyId = facultyId;
        Name = name.Trim();
        Description = description?.Trim();
        Location = location?.Trim();
        Dean = dean?.Trim();
        FechaRegistro = fechaRegistro ?? DateTime.Now;
        Activo = activo;

        // Validaciones de negocio
        ValidateBusinessRules();
    }

    /// <summary>
    /// Constructor para crear nueva facultad (sin ID)
    /// </summary>
    public Faculty(
        string name, 
        string? description = null, 
        string? location = null, 
        string? dean = null)
        : this(0, name, description, location, dean, DateTime.Now, true)
    {
    }

    /// <summary>
    /// Actualiza el nombre de la facultad manteniendo inmutabilidad
    /// </summary>
    public Faculty UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(newName));

        return new Faculty(FacultyId, newName, Description, Location, Dean, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza la descripción de la facultad manteniendo inmutabilidad
    /// </summary>
    public Faculty UpdateDescription(string? newDescription)
    {
        return new Faculty(FacultyId, Name, newDescription, Location, Dean, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza la ubicación de la facultad manteniendo inmutabilidad
    /// </summary>
    public Faculty UpdateLocation(string? newLocation)
    {
        return new Faculty(FacultyId, Name, Description, newLocation, Dean, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza el decano de la facultad manteniendo inmutabilidad
    /// </summary>
    public Faculty UpdateDean(string? newDean)
    {
        return new Faculty(FacultyId, Name, Description, Location, newDean, FechaRegistro, Activo);
    }

    /// <summary>
    /// Desactiva la facultad (soft delete)
    /// </summary>
    public Faculty Deactivate()
    {
        if (!Activo)
            throw new InvalidOperationException("La facultad ya está inactiva");

        return new Faculty(FacultyId, Name, Description, Location, Dean, FechaRegistro, false);
    }

    /// <summary>
    /// Reactiva la facultad
    /// </summary>
    public Faculty Activate()
    {
        if (Activo)
            throw new InvalidOperationException("La facultad ya está activa");

        return new Faculty(FacultyId, Name, Description, Location, Dean, FechaRegistro, true);
    }

    /// <summary>
    /// Calcula los años desde que se creó la facultad
    /// </summary>
    public int CalculateYearsSinceCreation(DateTime? referenceDate = null)
    {
        var reference = referenceDate ?? DateTime.Now;
        var years = reference.Year - FechaRegistro.Year;
        
        // Ajustar si aún no ha cumplido el aniversario este año
        if (reference.Month < FechaRegistro.Month || 
            (reference.Month == FechaRegistro.Month && reference.Day < FechaRegistro.Day))
        {
            years--;
        }
        
        return Math.Max(0, years);
    }

    /// <summary>
    /// Determina si la facultad puede recibir nuevas carreras (debe estar activa)
    /// </summary>
    public bool CanAddCareers()
    {
        return Activo;
    }

    /// <summary>
    /// Determina si tiene un decano asignado
    /// </summary>
    public bool HasDean()
    {
        return !string.IsNullOrWhiteSpace(Dean);
    }

    /// <summary>
    /// Determina si tiene ubicación específica
    /// </summary>
    public bool HasLocation()
    {
        return !string.IsNullOrWhiteSpace(Location);
    }

    /// <summary>
    /// Obtiene información resumida de la facultad
    /// </summary>
    public string GetFacultySummary()
    {
        var status = Activo ? "Activa" : "Inactiva";
        var age = CalculateYearsSinceCreation();
        var deanInfo = HasDean() ? $"Decano: {Dean}" : "Sin decano";
        
        return $"{Name} ({status} - {age} años, {deanInfo})";
    }

    /// <summary>
    /// Valida reglas de negocio de la facultad
    /// </summary>
    private void ValidateBusinessRules()
    {
        // Validar que la fecha de registro no sea futura
        var currentDate = DateTime.Now;
        if (FechaRegistro > currentDate.AddDays(1)) // Margen de 1 día por diferencias de zona horaria
            throw new ArgumentException("La fecha de registro no puede ser futura");
        
        // Validar longitud del nombre
        if (Name.Length < 3)
            throw new ArgumentException("El nombre de la facultad debe tener al menos 3 caracteres");
        
        if (Name.Length > 100)
            throw new ArgumentException("El nombre de la facultad no puede exceder 100 caracteres");
        
        // Validar descripción si se proporciona
        if (!string.IsNullOrEmpty(Description) && Description.Length > 1000)
            throw new ArgumentException("La descripción no puede exceder 1000 caracteres");

        // Validar ubicación si se proporciona
        if (!string.IsNullOrEmpty(Location) && Location.Length > 200)
            throw new ArgumentException("La ubicación no puede exceder 200 caracteres");

        // Validar decano si se proporciona
        if (!string.IsNullOrEmpty(Dean) && Dean.Length > 100)
            throw new ArgumentException("El nombre del decano no puede exceder 100 caracteres");
    }

    /// <summary>
    /// Representación string de la facultad
    /// </summary>
    public override string ToString()
    {
        return GetFacultySummary();
    }

    /// <summary>
    /// Equality basada en Name (identificador único de negocio)
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Faculty other)
            return false;
            
        return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// HashCode basado en Name
    /// </summary>
    public override int GetHashCode()
    {
        return Name.ToLowerInvariant().GetHashCode();
    }

    /// <summary>
    /// Operador de igualdad
    /// </summary>
    public static bool operator ==(Faculty? left, Faculty? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Operador de desigualdad
    /// </summary>
    public static bool operator !=(Faculty? left, Faculty? right)
    {
        return !Equals(left, right);
    }
}