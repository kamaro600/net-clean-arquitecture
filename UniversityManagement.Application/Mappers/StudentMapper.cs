using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entre StudentDomain y DTOs de aplicación
/// </summary>
public static class StudentMapper
{
    /// <summary>
    /// Convierte de StudentDomain a StudentResponse
    /// </summary>
    public static StudentResponse ToResponse(Student domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new StudentResponse
        {
            Id = domain.Id,
            FirstName = domain.FullName.FirstName,
            LastName = domain.FullName.LastName,
            Dni = domain.Dni.Value,
            Email = domain.Email.Value,
            Phone = domain.Phone?.Value,
            BirthDate = domain.Birthdate,
            Address = domain.Address?.ToString(),
            RegisterDate = domain.RegistrationDate,
            IsActive = domain.IsActive,
            Careers = new List<StudentCareerResponse>() // Por ahora vacío, se puede expandir después
        };
    }

    /// <summary>
    /// Convierte de StudentDomain a StudentResponse incluyendo las carreras relacionadas
    /// </summary>
    public static StudentResponse ToResponse(Student domain, IEnumerable<Career> careers)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        var careerList = careers.ToList();

        return new StudentResponse
        {
            Id = domain.Id,
            FirstName = domain.FullName.FirstName,
            LastName = domain.FullName.LastName,
            Dni = domain.Dni.Value,
            Email = domain.Email.Value,
            Phone = domain.Phone?.Value,
            BirthDate = domain.Birthdate,
            Address = domain.Address?.ToString(),
            RegisterDate = domain.RegistrationDate,
            IsActive = domain.IsActive,
            Careers = careerList.Select(c => c.ToStudentCareerData()).ToList()
        };
    }

    /// <summary>
    /// Convierte un Career a StudentCareerResponse
    /// </summary>
    public static StudentCareerResponse ToStudentCareerData(this Career career)
    {
        return new StudentCareerResponse
        {
            CareerId = career.CareerId,
            CareerName = career.Name,
            CareerDescription = career.Description,
            FacultyName = null, // Se debe obtener por separado si es necesario
            EnrollmentDate = DateTime.Now, // Se debe obtener del StudentCareer si está disponible
            IsActive = career.Activo
        };
    }

    /// <summary>
    /// Convierte una colección de StudentDomain a StudentResponse
    /// </summary>
    public static List<StudentResponse> ToResponseList(IEnumerable<Student> domains)
    {
        if (domains == null)
            return new List<StudentResponse>();

        return domains.Select(ToResponse).ToList();
    }
}