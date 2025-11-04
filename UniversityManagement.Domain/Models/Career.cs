namespace UniversityManagement.Domain.Models;

/// <summary>
/// Entidad de dominio pura para Carrera - Solo propiedades de dominio, sin dependencias de persistencia
/// </summary>
public class Career
{
    public int CareerId { get; }
    public int FacultyId { get; }
    public string Name { get; }
    public string? Description { get; }
    public int SemesterDuration { get; }
    public string? AwardedTitle { get; }
    public DateTime FechaRegistro { get; }
    public bool Activo { get; }

    /// <summary>
    /// Constructor principal para crear una carrera de dominio
    /// </summary>
    public Career(
        int careerId,
        int facultyId,
        string name,
        int semesterDuration,
        string? description = null,
        string? awardedTitle = null,
        DateTime? fechaRegistro = null,
        bool activo = true)
    {
        if (careerId < 0)
            throw new ArgumentException("El CareerId debe ser mayor o igual a 0", nameof(careerId));
        
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("El nombre de la carrera no puede estar vacío", nameof(name));
        
        if (facultyId <= 0)
            throw new ArgumentException("El FacultyId debe ser mayor a 0", nameof(facultyId));
        
        if (semesterDuration <= 0)
            throw new ArgumentException("La duración en semestres debe ser mayor a 0", nameof(semesterDuration));
        
        CareerId = careerId;
        FacultyId = facultyId;
        Name = name.Trim();
        Description = description?.Trim();
        SemesterDuration = semesterDuration;
        AwardedTitle = awardedTitle?.Trim();
        FechaRegistro = fechaRegistro ?? DateTime.Now;
        Activo = activo;

        // Validaciones de negocio
        ValidateBusinessRules();
    }

    /// <summary>
    /// Constructor para crear nueva carrera (sin ID)
    /// </summary>
    public Career(
        int facultyId,
        string name,
        int semesterDuration,
        string? description = null,
        string? awardedTitle = null)
        : this(0, facultyId, name, semesterDuration, description, awardedTitle, DateTime.Now, true)
    {
    }

    /// <summary>
    /// Actualiza el nombre de la carrera manteniendo inmutabilidad
    /// </summary>
    public Career UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(newName));

        return new Career(CareerId, FacultyId, newName, SemesterDuration, Description, AwardedTitle, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza la descripción de la carrera manteniendo inmutabilidad
    /// </summary>
    public Career UpdateDescription(string? newDescription)
    {
        return new Career(CareerId, FacultyId, Name, SemesterDuration, newDescription, AwardedTitle, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza la duración en semestres de la carrera manteniendo inmutabilidad
    /// </summary>
    public Career UpdateSemesterDuration(int newSemesterDuration)
    {
        if (newSemesterDuration <= 0)
            throw new ArgumentException("La duración en semestres debe ser mayor a 0", nameof(newSemesterDuration));

        return new Career(CareerId, FacultyId, Name, newSemesterDuration, Description, AwardedTitle, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza el título otorgado manteniendo inmutabilidad
    /// </summary>
    public Career UpdateAwardedTitle(string? newAwardedTitle)
    {
        return new Career(CareerId, FacultyId, Name, SemesterDuration, Description, newAwardedTitle, FechaRegistro, Activo);
    }

    /// <summary>
    /// Cambia la carrera a otra facultad manteniendo inmutabilidad
    /// </summary>
    public Career TransferToFaculty(int newFacultyId)
    {
        if (newFacultyId <= 0)
            throw new ArgumentException("El FacultyId debe ser mayor a 0", nameof(newFacultyId));

        return new Career(CareerId, newFacultyId, Name, SemesterDuration, Description, AwardedTitle, FechaRegistro, Activo);
    }

    /// <summary>
    /// Desactiva la carrera (soft delete)
    /// </summary>
    public Career Deactivate()
    {
        if (!Activo)
            throw new InvalidOperationException("La carrera ya está inactiva");

        return new Career(CareerId, FacultyId, Name, SemesterDuration, Description, AwardedTitle, FechaRegistro, false);
    }

    /// <summary>
    /// Reactiva la carrera
    /// </summary>
    public Career Activate()
    {
        if (Activo)
            throw new InvalidOperationException("La carrera ya está activa");

        return new Career(CareerId, FacultyId, Name, SemesterDuration, Description, AwardedTitle, FechaRegistro, true);
    }

    /// <summary>
    /// Calcula la duración en años (asumiendo 2 semestres por año)
    /// </summary>
    public double CalculateDurationInYears()
    {
        return SemesterDuration / 2.0;
    }

    /// <summary>
    /// Determina si es una carrera de grado (8-10 semestres / 4-5 años)
    /// </summary>
    public bool IsUndergraduateCareer()
    {
        return SemesterDuration >= 8 && SemesterDuration <= 10;
    }

    /// <summary>
    /// Determina si es una carrera de posgrado (2-6 semestres / 1-3 años)
    /// </summary>
    public bool IsPostgraduateCareer()
    {
        return SemesterDuration >= 2 && SemesterDuration <= 6;
    }

    /// <summary>
    /// Determina si es una carrera extendida (más de 10 semestres)
    /// </summary>
    public bool IsExtendedCareer()
    {
        return SemesterDuration > 10;
    }

    /// <summary>
    /// Calcula los años desde que se creó la carrera
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
    /// Determina si la carrera permite inscripciones (debe estar activa)
    /// </summary>
    public bool AllowsEnrollment()
    {
        return Activo;
    }

    /// <summary>
    /// Determina si tiene un título específico definido
    /// </summary>
    public bool HasAwardedTitle()
    {
        return !string.IsNullOrWhiteSpace(AwardedTitle);
    }

    /// <summary>
    /// Obtiene información resumida de la carrera
    /// </summary>
    public string GetCareerSummary()
    {
        var typeInfo = IsUndergraduateCareer() ? "Grado" : 
                      IsPostgraduateCareer() ? "Posgrado" : 
                      "Extendida";
        
        var years = CalculateDurationInYears();
        return $"{Name} ({SemesterDuration} semestres / {years} años - {typeInfo})";
    }

    /// <summary>
    /// Valida reglas de negocio de la carrera
    /// </summary>
    private void ValidateBusinessRules()
    {
        // Validar que la fecha de registro no sea futura
        var currentDate = DateTime.Now;
        if (FechaRegistro > currentDate.AddDays(1)) // Margen de 1 día por diferencias de zona horaria
            throw new ArgumentException("La fecha de registro no puede ser futura");
        
        // Validar longitud del nombre
        if (Name.Length < 3)
            throw new ArgumentException("El nombre de la carrera debe tener al menos 3 caracteres");
        
        if (Name.Length > 100)
            throw new ArgumentException("El nombre de la carrera no puede exceder 100 caracteres");
        
        // Validar descripción si se proporciona
        if (!string.IsNullOrEmpty(Description) && Description.Length > 500)
            throw new ArgumentException("La descripción no puede exceder 500 caracteres");

        // Validar título otorgado si se proporciona
        if (!string.IsNullOrEmpty(AwardedTitle) && AwardedTitle.Length > 150)
            throw new ArgumentException("El título otorgado no puede exceder 150 caracteres");

        // Validar duración razonable (máximo 20 semestres / 10 años)
        if (SemesterDuration > 20)
            throw new ArgumentException("La duración no puede exceder 20 semestres");
    }

    /// <summary>
    /// Representación string de la carrera
    /// </summary>
    public override string ToString()
    {
        return GetCareerSummary();
    }

    /// <summary>
    /// Equality basada en Name y FacultyId (identificador único de negocio)
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Career other)
            return false;
            
        return Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase) && 
               FacultyId == other.FacultyId;
    }

    /// <summary>
    /// HashCode basado en Name y FacultyId
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(Name.ToLowerInvariant(), FacultyId);
    }

    /// <summary>
    /// Operador de igualdad
    /// </summary>
    public static bool operator ==(Career? left, Career? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Operador de desigualdad
    /// </summary>
    public static bool operator !=(Career? left, Career? right)
    {
        return !Equals(left, right);
    }
}