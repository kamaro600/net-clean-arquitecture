namespace UniversityManagement.Domain.Models;

/// <summary>
/// Entidad de dominio para la relación many-to-many entre Professor y Career
/// </summary>
public class ProfessorCareer
{
    public int ProfessorId { get; }
    public int CareerId { get; }
    public DateTime AssignmentDate { get; }
    public bool IsActive { get; }

    /// <summary>
    /// Constructor principal para crear una asignación profesor-carrera
    /// </summary>
    public ProfessorCareer(
        int professorId,
        int careerId,
        DateTime? assignmentDate = null,
        bool isActive = true)
    {
        if (professorId <= 0)
            throw new ArgumentException("El ProfessorId debe ser mayor a 0", nameof(professorId));
        
        if (careerId <= 0)
            throw new ArgumentException("El CareerId debe ser mayor a 0", nameof(careerId));
        
        ProfessorId = professorId;
        CareerId = careerId;
        AssignmentDate = assignmentDate ?? DateTime.UtcNow;
        IsActive = isActive;

        // Validaciones de negocio
        ValidateBusinessRules();
    }

    /// <summary>
    /// Constructor para crear nueva asignación (sin ID)
    /// </summary>
    public ProfessorCareer(
        int professorId,
        int careerId)
        : this(professorId, careerId, DateTime.UtcNow, true)
    {
    }

    /// <summary>
    /// Desactiva la asignación (termina la relación profesor-carrera)
    /// </summary>
    public ProfessorCareer Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("La asignación ya está inactiva");

        return new ProfessorCareer(ProfessorId, CareerId, AssignmentDate, false);
    }

    /// <summary>
    /// Reactiva la asignación
    /// </summary>
    public ProfessorCareer Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("La asignación ya está activa");

        return new ProfessorCareer(ProfessorId, CareerId, AssignmentDate, true);
    }

    /// <summary>
    /// Calcula los años desde que se realizó la asignación
    /// </summary>
    public int CalculateYearsAssigned(DateTime? referenceDate = null)
    {
        var reference = referenceDate ?? DateTime.Now;
        var years = reference.Year - AssignmentDate.Year;
        
        // Ajustar si aún no ha cumplido el aniversario este año
        if (reference.Month < AssignmentDate.Month || 
            (reference.Month == AssignmentDate.Month && reference.Day < AssignmentDate.Day))
        {
            years--;
        }
        
        return Math.Max(0, years);
    }

    /// <summary>
    /// Determina si es una asignación nueva (menos de 1 año)
    /// </summary>
    public bool IsNewAssignment()
    {
        return CalculateYearsAssigned() < 1;
    }

    /// <summary>
    /// Determina si es una asignación de larga duración (5 años o más)
    /// </summary>
    public bool IsLongTermAssignment()
    {
        return CalculateYearsAssigned() >= 5;
    }

    /// <summary>
    /// Obtiene información resumida de la asignación
    /// </summary>
    public string GetAssignmentSummary()
    {
        var status = IsActive ? "Activa" : "Inactiva";
        var duration = CalculateYearsAssigned();
        var durationText = IsNewAssignment() ? "Nueva" : 
                          IsLongTermAssignment() ? "Larga duración" : 
                          "Establecida";
        
        return $"Profesor {ProfessorId} → Carrera {CareerId} ({status} - {duration} años, {durationText})";
    }

    /// <summary>
    /// Valida reglas de negocio de la asignación
    /// </summary>
    private void ValidateBusinessRules()
    {
        // Validar que la fecha de asignación no sea futura
        var currentDate = DateTime.UtcNow;
        if (AssignmentDate > currentDate.AddDays(1)) // Margen de 1 día por diferencias de zona horaria
            throw new ArgumentException("La fecha de asignación no puede ser futura");
    }

    /// <summary>
    /// Representación string de la asignación
    /// </summary>
    public override string ToString()
    {
        return GetAssignmentSummary();
    }

    /// <summary>
    /// Equality basada en ProfessorId y CareerId (identificador único de negocio)
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not ProfessorCareer other)
            return false;
            
        return ProfessorId == other.ProfessorId && CareerId == other.CareerId;
    }

    /// <summary>
    /// HashCode basado en ProfessorId y CareerId
    /// </summary>
    public override int GetHashCode()
    {
        return HashCode.Combine(ProfessorId, CareerId);
    }

    /// <summary>
    /// Operador de igualdad
    /// </summary>
    public static bool operator ==(ProfessorCareer? left, ProfessorCareer? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Operador de desigualdad
    /// </summary>
    public static bool operator !=(ProfessorCareer? left, ProfessorCareer? right)
    {
        return !Equals(left, right);
    }
}