namespace UniversityManagement.Domain.Models;

/// <summary>
/// Entidad de dominio pura para Profesor - Solo propiedades de dominio, sin dependencias de persistencia
/// </summary>
public class Professor
{
    public int ProfessorId { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string Dni { get; }
    public string Email { get; }
    public string? Phone { get; }
    public string? Specialty { get; }
    public string? AcademicDegree { get; }
    public DateTime FechaRegistro { get; }
    public bool Activo { get; }

    /// <summary>
    /// Constructor principal para crear un profesor de dominio
    /// </summary>
    public Professor(
        int professorId,
        string firstName,
        string lastName,
        string dni,
        string email,
        string? phone = null,
        string? specialty = null,
        string? academicDegree = null,
        DateTime? fechaRegistro = null,
        bool activo = true)
    {
        if (professorId < 0)
            throw new ArgumentException("El ProfessorId debe ser mayor o igual a 0", nameof(professorId));
        
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(firstName));
        
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido no puede estar vacío", nameof(lastName));
        
        if (string.IsNullOrWhiteSpace(dni))
            throw new ArgumentException("El DNI no puede estar vacío", nameof(dni));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("El email no puede estar vacío", nameof(email));
        
        ProfessorId = professorId;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Dni = dni.Trim();
        Email = email.Trim();
        Phone = phone?.Trim();
        Specialty = specialty?.Trim();
        AcademicDegree = academicDegree?.Trim();
        FechaRegistro = fechaRegistro ?? DateTime.Now;
        Activo = activo;

        // Validaciones de negocio
        ValidateBusinessRules();
    }

    /// <summary>
    /// Constructor para crear nuevo profesor (sin ID)
    /// </summary>
    public Professor(
        string firstName,
        string lastName,
        string dni,
        string email,
        string? phone = null,
        string? specialty = null,
        string? academicDegree = null)
        : this(0, firstName, lastName, dni, email, phone, specialty, academicDegree, DateTime.Now, true)
    {
    }

    /// <summary>
    /// Obtiene el nombre completo del profesor
    /// </summary>
    public string GetFullName()
    {
        return $"{FirstName} {LastName}";
    }

    /// <summary>
    /// Actualiza el nombre manteniendo inmutabilidad
    /// </summary>
    public Professor UpdateFirstName(string newFirstName)
    {
        if (string.IsNullOrWhiteSpace(newFirstName))
            throw new ArgumentException("El nombre no puede estar vacío", nameof(newFirstName));

        return new Professor(ProfessorId, newFirstName, LastName, Dni, Email, Phone, Specialty, AcademicDegree, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza el apellido manteniendo inmutabilidad
    /// </summary>
    public Professor UpdateLastName(string newLastName)
    {
        if (string.IsNullOrWhiteSpace(newLastName))
            throw new ArgumentException("El apellido no puede estar vacío", nameof(newLastName));

        return new Professor(ProfessorId, FirstName, newLastName, Dni, Email, Phone, Specialty, AcademicDegree, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza el email manteniendo inmutabilidad
    /// </summary>
    public Professor UpdateEmail(string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            throw new ArgumentException("El email no puede estar vacío", nameof(newEmail));

        return new Professor(ProfessorId, FirstName, LastName, Dni, newEmail, Phone, Specialty, AcademicDegree, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza el teléfono manteniendo inmutabilidad
    /// </summary>
    public Professor UpdatePhone(string? newPhone)
    {
        return new Professor(ProfessorId, FirstName, LastName, Dni, Email, newPhone, Specialty, AcademicDegree, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza la especialidad manteniendo inmutabilidad
    /// </summary>
    public Professor UpdateSpecialty(string? newSpecialty)
    {
        return new Professor(ProfessorId, FirstName, LastName, Dni, Email, Phone, newSpecialty, AcademicDegree, FechaRegistro, Activo);
    }

    /// <summary>
    /// Actualiza el grado académico manteniendo inmutabilidad
    /// </summary>
    public Professor UpdateAcademicDegree(string? newAcademicDegree)
    {
        return new Professor(ProfessorId, FirstName, LastName, Dni, Email, Phone, Specialty, newAcademicDegree, FechaRegistro, Activo);
    }

    /// <summary>
    /// Desactiva el profesor (soft delete)
    /// </summary>
    public Professor Deactivate()
    {
        if (!Activo)
            throw new InvalidOperationException("El profesor ya está inactivo");

        return new Professor(ProfessorId, FirstName, LastName, Dni, Email, Phone, Specialty, AcademicDegree, FechaRegistro, false);
    }

    /// <summary>
    /// Reactiva el profesor
    /// </summary>
    public Professor Activate()
    {
        if (Activo)
            throw new InvalidOperationException("El profesor ya está activo");

        return new Professor(ProfessorId, FirstName, LastName, Dni, Email, Phone, Specialty, AcademicDegree, FechaRegistro, true);
    }

    /// <summary>
    /// Calcula los años de experiencia desde la fecha de registro
    /// </summary>
    public int CalculateYearsOfExperience(DateTime? referenceDate = null)
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
    /// Determina si es un profesor senior (5 años o más de experiencia)
    /// </summary>
    public bool IsSeniorProfessor()
    {
        return CalculateYearsOfExperience() >= 5;
    }

    /// <summary>
    /// Determina si tiene especialidad definida
    /// </summary>
    public bool HasSpecialty()
    {
        return !string.IsNullOrWhiteSpace(Specialty);
    }

    /// <summary>
    /// Determina si tiene grado académico definido
    /// </summary>
    public bool HasAcademicDegree()
    {
        return !string.IsNullOrWhiteSpace(AcademicDegree);
    }

    /// <summary>
    /// Determina si puede enseñar en una carrera específica basado en su especialidad
    /// </summary>
    public bool CanTeachInCareer(string careerName)
    {
        if (string.IsNullOrWhiteSpace(careerName) || !HasSpecialty())
            return false;

        // Lógica simple: si la especialidad está relacionada con el nombre de la carrera
        return Specialty!.Contains(careerName, StringComparison.OrdinalIgnoreCase) ||
               careerName.Contains(Specialty!, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Obtiene información resumida del profesor
    /// </summary>
    public string GetProfessorSummary()
    {
        var status = Activo ? "Activo" : "Inactivo";
        var experience = CalculateYearsOfExperience();
        var seniorStatus = IsSeniorProfessor() ? "Senior" : "Junior";
        
        return $"{GetFullName()} ({status} - {experience} años, {seniorStatus})";
    }

    /// <summary>
    /// Valida reglas de negocio del profesor
    /// </summary>
    private void ValidateBusinessRules()
    {
        // Validar formato de email básico
        if (!Email.Contains("@"))
            throw new ArgumentException("El email debe tener un formato válido");

        // Validar que la fecha de registro no sea futura
        var currentDate = DateTime.Now;
        if (FechaRegistro > currentDate.AddDays(1)) // Margen de 1 día por diferencias de zona horaria
            throw new ArgumentException("La fecha de registro no puede ser futura");
        
        // Validar longitud del DNI
        if (Dni.Length < 7 || Dni.Length > 12)
            throw new ArgumentException("El DNI debe tener entre 7 y 12 caracteres");
        
        // Validar longitudes de campos
        if (FirstName.Length > 50)
            throw new ArgumentException("El nombre no puede exceder 50 caracteres");
        
        if (LastName.Length > 50)
            throw new ArgumentException("El apellido no puede exceder 50 caracteres");
        
        if (Email.Length > 100)
            throw new ArgumentException("El email no puede exceder 100 caracteres");
        
        if (!string.IsNullOrEmpty(Phone) && Phone.Length > 20)
            throw new ArgumentException("El teléfono no puede exceder 20 caracteres");
        
        if (!string.IsNullOrEmpty(Specialty) && Specialty.Length > 100)
            throw new ArgumentException("La especialidad no puede exceder 100 caracteres");
        
        if (!string.IsNullOrEmpty(AcademicDegree) && AcademicDegree.Length > 100)
            throw new ArgumentException("El grado académico no puede exceder 100 caracteres");
    }

    /// <summary>
    /// Representación string del profesor
    /// </summary>
    public override string ToString()
    {
        return GetProfessorSummary();
    }

    /// <summary>
    /// Equality basada en DNI (identificador único de negocio)
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Professor other)
            return false;
            
        return Dni.Equals(other.Dni, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// HashCode basado en DNI
    /// </summary>
    public override int GetHashCode()
    {
        return Dni.ToLowerInvariant().GetHashCode();
    }

    /// <summary>
    /// Operador de igualdad
    /// </summary>
    public static bool operator ==(Professor? left, Professor? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Operador de desigualdad
    /// </summary>
    public static bool operator !=(Professor? left, Professor? right)
    {
        return !Equals(left, right);
    }
}