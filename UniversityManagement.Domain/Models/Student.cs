using UniversityManagement.Domain.Common;
using UniversityManagement.Domain.Models.ValueObjects;

namespace UniversityManagement.Domain.Models;

public class Student : BaseEntity
{
    public int EstudianteId { get; set; }
    
    // Propiedades primitivas para EF Core (principales)
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Dni { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public DateTime FechaNacimiento { get; set; }
    public string? Direccion { get; set; }

    // Value Objects para lógica de dominio
    public FullName? FullNameVO { get; private set; }
    public Dni? DniVO { get; private set; }
    public Email? EmailVO { get; private set; }
    public Phone? PhoneVO { get; private set; }
    public Address? AddressVO { get; private set; }

    // Propiedades de navegación
    public virtual ICollection<StudentCareer> StudentCareers { get; set; } = new List<StudentCareer>();

    // Constructor para EF Core
    public Student() 
    {
        SyncValueObjects();
    }

    // Método para crear Value Objects desde propiedades primitivas
    public void SyncValueObjects()
    {
        try
        {
            if (!string.IsNullOrWhiteSpace(Nombre) && !string.IsNullOrWhiteSpace(Apellido))
                FullNameVO = new FullName(Nombre, Apellido);
            
            if (!string.IsNullOrWhiteSpace(Dni))
                DniVO = new Dni(Dni);
            
            if (!string.IsNullOrWhiteSpace(Email))
                EmailVO = new Email(Email);
            
            if (!string.IsNullOrWhiteSpace(Telefono))
                PhoneVO = new Phone(Telefono);
            
            if (!string.IsNullOrWhiteSpace(Direccion))
                AddressVO = new Address(Direccion);
        }
        catch
        {
            // Si hay error en validación, mantenemos valores primitivos
            // y dejamos Value Objects como null
        }
    }

    // Método para validar datos usando Value Objects
    public void ValidateStudentData()
    {
        // Esto forzará la validación a través de Value Objects
        var fullName = new FullName(Nombre, Apellido);
        var dni = new Dni(Dni);
        var email = new Email(Email);
        
        if (!string.IsNullOrWhiteSpace(Telefono))
            new Phone(Telefono);
        
        if (!string.IsNullOrWhiteSpace(Direccion))
            new Address(Direccion);

        // Si llegamos aquí, todo es válido, actualizamos los Value Objects
        FullNameVO = fullName;
        DniVO = dni;
        EmailVO = email;
        
        if (!string.IsNullOrWhiteSpace(Telefono))
            PhoneVO = new Phone(Telefono);
        
        if (!string.IsNullOrWhiteSpace(Direccion))
            AddressVO = new Address(Direccion);
    }
}