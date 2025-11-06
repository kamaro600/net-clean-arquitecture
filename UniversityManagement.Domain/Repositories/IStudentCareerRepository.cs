using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

/// <summary>
/// Interfaz para el repositorio de matriculación (relación estudiante-carrera)
/// </summary>
public interface IStudentCareerRepository
{
    Task<StudentCareer?> GetEnrollmentAsync(int studentId, int careerId);
    Task<IEnumerable<StudentCareer>> GetStudentEnrollmentsAsync(int studentId);
    Task<IEnumerable<StudentCareer>> GetCareerEnrollmentsAsync(int careerId);
    Task<StudentCareer> AddEnrollmentAsync(StudentCareer enrollment);
    Task<StudentCareer> UpdateEnrollmentAsync(StudentCareer enrollment);
    Task<bool> DeleteEnrollmentAsync(int studentId, int careerId);
    Task<bool> ExistsActiveEnrollmentAsync(int studentId, int careerId);
    Task<int> GetActiveEnrollmentCountAsync(int studentId);
}