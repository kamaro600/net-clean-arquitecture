using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Mappers;

/// <summary>
/// Mapper para convertir entidades Student a DTOs de respuesta
/// </summary>
public static class StudentMapper
{  
    /// <summary>
    /// Convierte Student a StudentData para respuestas paginadas
    /// </summary>
    public static StudentResponse ToStudentData(this Student student)
    {
        return new StudentResponse
        {
            Id = student.EstudianteId,
            Nombre = student.Nombre,
            Apellido = student.Apellido,
            Dni = student.Dni,
            Email = student.Email,
            Telefono = student.Telefono,
            FechaNacimiento = student.FechaNacimiento,
            Direccion = student.Direccion,
            Activo = student.Activo,
            FechaRegistro = student.FechaRegistro,
            Carreras = student.StudentCareers?.Select(sc => new StudentCareerResponse
            {
                CarreraId = sc.Career?.CarreraId ?? 0,
                NombreCarrera = sc.Career?.Nombre ?? string.Empty,
                DescripcionCarrera = sc.Career?.Descripcion,
                FacultadNombre = sc.Career?.Faculty?.Nombre,
                FechaInscripcion = sc.FechaInscripcion,
                Activo = sc.Activo
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