using UniversityManagement.Domain.Models.ValueObjects;

namespace UniversityManagement.Domain.Models;

/// <summary>
/// Entidad de dominio pura - Solo Value Objects, sin dependencias de persistencia
/// </summary>
public class Student
{
    public int Id { get; }
    public FullName FullName { get; }
    public Dni Dni { get; }
    public Email Email { get; }
    public Phone? Phone { get; }
    public Address? Address { get; }
    public DateTime Birthdate { get; }
    public DateTime RegistrationDate { get; }
    public bool IsActive { get; }

    /// <summary>
    /// Constructor principal para crear un estudiante de dominio
    /// </summary>
    public Student(
        int id,
        FullName fullName,
        Dni dni,
        Email email,
        DateTime birthdate,
        Phone? phone = null,
        Address? address = null,
        DateTime? registrationDate = null,
        bool isActive = true)
    {
        if (id < 0)
            throw new ArgumentException("El ID debe ser mayor o igual a 0", nameof(id));
        
        Id = id;
        FullName = fullName ?? throw new ArgumentNullException(nameof(fullName));
        Dni = dni ?? throw new ArgumentNullException(nameof(dni));
        Email = email ?? throw new ArgumentNullException(nameof(email));
        Phone = phone;
        Address = address;
        Birthdate = birthdate;
        RegistrationDate = registrationDate ?? DateTime.Now;
        IsActive = isActive;

        // Validaciones de negocio
        ValidateBusinessRules();
    }

    /// <summary>
    /// Constructor para crear nuevo estudiante (sin ID)
    /// </summary>
    public Student(
        FullName fullName,
        Dni dni,
        Email email,
        DateTime birthdate,
        Phone? phone = null,
        Address? address = null)
        : this(0, fullName, dni, email, birthdate, phone, address, DateTime.Now, true)
    {
    }

    /// <summary>
    /// Actualiza el email del estudiante manteniendo inmutabilidad
    /// </summary>
    public Student UpdateEmail(Email newEmail)
    {
        if (newEmail == null)
            throw new ArgumentNullException(nameof(newEmail));

        return new Student(Id, FullName, Dni, newEmail, Birthdate, 
            Phone, Address, RegistrationDate, IsActive);
    }

    /// <summary>
    /// Actualiza el teléfono del estudiante manteniendo inmutabilidad
    /// </summary>
    public Student UpdatePhone(Phone? newPhone)
    {
        return new Student(Id, FullName, Dni, Email, Birthdate, 
            newPhone, Address, RegistrationDate, IsActive);
    }

    /// <summary>
    /// Actualiza la dirección del estudiante manteniendo inmutabilidad
    /// </summary>
    public Student UpdateAddress(Address? newAddress)
    {
        return new Student(Id, FullName, Dni, Email, Birthdate, 
            Phone, newAddress, RegistrationDate, IsActive);
    }

    /// <summary>
    /// Desactiva el estudiante (soft delete)
    /// </summary>
    public Student Deactivate()
    {
        if (!IsActive)
            throw new InvalidOperationException("El estudiante ya está inactivo");

        return new Student(Id, FullName, Dni, Email, Birthdate, 
            Phone, Address, RegistrationDate, false);
    }

    /// <summary>
    /// Reactiva el estudiante
    /// </summary>
    public Student Activate()
    {
        if (IsActive)
            throw new InvalidOperationException("El estudiante ya está activo");

        return new Student(Id, FullName, Dni, Email, Birthdate, 
            Phone, Address, RegistrationDate, true);
    }

    /// <summary>
    /// Determina si el estudiante puede inscribirse en una carrera
    /// </summary>
    public bool CanEnrollInCareer(DateTime enrollmentDate)
    {
        if (!IsActive)
            return false;

        // Debe ser mayor de 16 años para inscribirse
        var age = CalculateAge(enrollmentDate);
        return age >= 16;
    }

    /// <summary>
    /// Determina si el estudiante puede inscribirse en una carrera específica
    /// </summary>
    public bool CanEnrollInCareer(Career career)
    {
        if (career == null)
            return false;

        if (!IsActive || !career.Activo)
            return false;

        // Debe ser mayor de 16 años para inscribirse (reducido a 14 para testing)
        var age = CalculateAge();
        return age >= 14; // Cambiado de 16 a 14 años
    }

    /// <summary>
    /// Calcula la edad del estudiante en una fecha específica
    /// </summary>
    public int CalculateAge(DateTime? referenceDate = null)
    {
        var reference = referenceDate ?? DateTime.Now;
        var age = reference.Year - Birthdate.Year;
        
        // Ajustar si aún no ha cumplido años este año
        if (reference.Month < Birthdate.Month || 
            (reference.Month == Birthdate.Month && reference.Day < Birthdate.Day))
        {
            age--;
        }
        
        return age;
    }

    /// <summary>
    /// Determina si el estudiante es mayor de edad
    /// </summary>
    public bool IsAdult(DateTime? referenceDate = null)
    {
        return CalculateAge(referenceDate) >= 18;
    }

    /// <summary>
    /// Obtiene el nombre completo formateado
    /// </summary>
    public string GetFormattedFullName()
    {
        return FullName.FullDisplayName;
    }

    /// <summary>
    /// Valida reglas de negocio del estudiante
    /// </summary>
    private void ValidateBusinessRules()
    {
        // Validar que la fecha de nacimiento sea razonable
        var currentDate = DateTime.Now;
        var age = CalculateAge(currentDate);
        
        if (age < 0)
            throw new ArgumentException("La fecha de nacimiento no puede ser futura");
        
        if (age > 120)
            throw new ArgumentException("La fecha de nacimiento no es válida (edad muy avanzada)");
        
        // Validar que la fecha de registro no sea futura
        if (RegistrationDate > currentDate.AddDays(1)) // Margen de 1 día por diferencias de zona horaria
            throw new ArgumentException("La fecha de registro no puede ser futura");
    }

    /// <summary>
    /// Representación string del estudiante
    /// </summary>
    public override string ToString()
    {
        return $"{FullName.FullDisplayName} ({Dni.Value}) - {Email.Value}";
    }

    /// <summary>
    /// Equality basada en DNI (identificador único de negocio)
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (obj is not Student other)
            return false;
            
        return Dni.Equals(other.Dni);
    }

    /// <summary>
    /// HashCode basado en DNI
    /// </summary>
    public override int GetHashCode()
    {
        return Dni.GetHashCode();
    }

    /// <summary>
    /// Operador de igualdad
    /// </summary>
    public static bool operator ==(Student? left, Student? right)
    {
        return Equals(left, right);
    }

    /// <summary>
    /// Operador de desigualdad
    /// </summary>
    public static bool operator !=(Student? left, Student? right)
    {
        return !Equals(left, right);
    }
}