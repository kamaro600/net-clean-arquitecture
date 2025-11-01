namespace UniversityManagement.Domain.Models;

/// <summary>
/// Entidad de dominio para la relación many-to-many entre Student y Career
/// Mantiene las mismas propiedades que la entidad StudentCareer original
/// </summary>
public class StudentCareer
{
    public int StudentId { get; }
    public int CareerId { get; }
    public DateTime EnrollmentDate { get; }
    public bool IsActive { get; }

    /// <summary>
    /// Constructor principal para crear una inscripción estudiante-carrera
    /// </summary>
    public StudentCareer(
        int studentId,
        int careerId,
        DateTime? enrollmentDate = null,
        bool isActive = true)
    {
        if (studentId <= 0)
            throw new ArgumentException("El StudentId debe ser mayor a 0", nameof(studentId));
        
        if (careerId <= 0)
            throw new ArgumentException("El CareerId debe ser mayor a 0", nameof(careerId));
        
        StudentId = studentId;
        CareerId = careerId;
        EnrollmentDate = enrollmentDate ?? DateTime.UtcNow;
        IsActive = isActive;

        // Validaciones de negocio
        ValidateBusinessRules();
    }

    /// <summary>
    /// Constructor para crear nueva inscripción
    /// </summary>
    public StudentCareer(
        int studentId,
        int careerId)
        : this(studentId, careerId, DateTime.UtcNow, true)
    {
    }

    /// <summary>
    /// Desactiva la inscripción (termina la relación estudiante-carrera)
    /// </summary>
    public StudentCareer Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La inscripción ya está inactiva");

        return new StudentCareer(StudentId, CareerId, EnrollmentDate, false);
    }

    /// <summary>
    /// Reactiva la inscripción
    /// </summary>
    public StudentCareer Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("La inscripción ya está activa");

        return new StudentCareer(StudentId, CareerId, EnrollmentDate, true);
    }

    /// <summary>
    /// Calcula los años desde que se realizó la inscripción
    /// </summary>
    public int CalculateYearsEnrolled(DateTime? referenceDate = null)
    {
        var reference = referenceDate ?? DateTime.UtcNow;
        var years = reference.Year - EnrollmentDate.Year;
        
        // Ajustar si aún no ha cumplido el aniversario este año
        if (reference.Month < EnrollmentDate.Month || 
            (reference.Month == EnrollmentDate.Month && reference.Day < EnrollmentDate.Day))
        {
            years--;
        }
        
        return Math.Max(0, years);
    }

    /// <summary>
    /// Determina si es una inscripción nueva (menos de 1 año)
    /// </summary>
    public bool IsNewEnrollment()
    {
        return CalculateYearsEnrolled() < 1;
    }

    /// <summary>
    /// Determina si es una inscripción de larga duración (5 años o más)
    /// </summary>
    public bool IsLongTermEnrollment()
    {
        return CalculateYearsEnrolled() >= 5;
    }

    /// <summary>
    /// Obtiene información resumida de la inscripción
    /// </summary>
    public string GetEnrollmentSummary()
    {
        var status = IsActive ? "Activa" : "Inactiva";
        var duration = CalculateYearsEnrolled();
        var durationText = IsNewEnrollment() ? "Nueva" : 
                          IsLongTermEnrollment() ? "Larga duración" : 
                          "En progreso";
        
        return $"Estudiante {StudentId} → Carrera {CareerId} ({status} - {duration} años, {durationText})";
    }

    /// <summary>
    /// Valida reglas de negocio de la inscripción
    /// </summary>
    private void ValidateBusinessRules()
    {
        // Validar que la fecha de inscripción no sea futura
        var currentDate = DateTime.UtcNow;
        if (EnrollmentDate > currentDate.AddDays(1)) // Margen de 1 día por diferencias de zona horaria
            throw new ArgumentException("La fecha de inscripción no puede ser futura");
    }

    /// <summary>
    /// Representación string de la inscripción
    /// </summary>
    public override string ToString()
    {
        return GetEnrollmentSummary();
    }

    /// <summary>
    /// Equality basada en StudentId y CareerId (identificador único de negocio)
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not StudentCareer other)
            return false;
            
        return StudentId == other.StudentId && CareerId == other.CareerId;
    }

    /// <summary>
    /// HashCode basado en StudentId y CareerId
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(StudentId, CareerId);
    }

    /// <summary>
    /// Operador de igualdad
    /// </summary>
    public static bool operator ==(StudentCareer? left, StudentCareer? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Operador de desigualdad
    /// </summary>
    public static bool operator !=(StudentCareer? left, StudentCareer? right)
    {
        return !Equals(left, right);
    }
}