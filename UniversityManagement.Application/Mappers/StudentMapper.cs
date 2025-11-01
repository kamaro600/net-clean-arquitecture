using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Student a DTOs de respuesta
/// </summary>
public static class StudentMapper
{
    /// <summary>
    /// Convierte Student a StudentResponse
    /// </summary>
    public static StudentResponse ToStudentData(this Student student)
    {
        return new StudentResponse
        {
            Id = student.EstudianteId,
            FirstName = student.Nombre,
            LastName = student.Apellido,
            Dni = student.Dni,
            Email = student.Email,
            Phone = student.Telefono,
            BirthDate = student.FechaNacimiento,
            Address = student.Direccion,
            IsActive = student.Activo,
            RegisterDate = student.FechaRegistro,
            Careers = student.StudentCareers?.Select(sc => new StudentCareerResponse
            {
                CareerId = sc.Career?.CareerId ?? 0,
                CareerName = sc.Career?.Name ?? string.Empty,
                CareerDescription = sc.Career?.Description,
                FacultyName = sc.Career?.Faculty?.Name,
                EnrollmentDate = sc.EnrollmentDate,
                IsActive = sc.IsActive
            }).ToList() ?? new List<StudentCareerResponse>()
        };
    }

    /// <summary>
    /// Convierte una lista de Student a lista de StudentResponse
    /// </summary>
    public static List<StudentResponse> ToStudentDataList(this IEnumerable<Student> students)
    {
        return students.Select(p => p.ToStudentData()).ToList();
    }
}