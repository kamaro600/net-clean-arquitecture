using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Models.ValueObjects;

namespace UniversityManagement.Domain.Repositories;

/// <summary>
/// Interfaz del repositorio de estudiantes - Trabaja con entidades de dominio puras
/// </summary>
public interface IStudentRepository
{
    /// <summary>
    /// Obtiene todos los estudiantes activos
    /// </summary>
    Task<IEnumerable<Student>> GetAllAsync();
    
    /// <summary>
    /// Obtiene un estudiante por su ID
    /// </summary>
    Task<Student?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtiene un estudiante por su DNI (Value Object)
    /// </summary>
    Task<Student?> GetByDniAsync(Dni dni);
    
    /// <summary>
    /// Obtiene un estudiante por su email (Value Object)
    /// </summary>
    Task<Student?> GetByEmailAsync(Email email);
    
    /// <summary>
    /// Crea un nuevo estudiante
    /// </summary>
    Task<Student> CreateAsync(Student student);
    
    /// <summary>
    /// Actualiza un estudiante existente
    /// </summary>
    Task<Student> UpdateAsync(Student student);
    
    /// <summary>
    /// Elimina un estudiante (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Verifica si existe un estudiante con el DNI especificado
    /// </summary>
    Task<bool> ExistsByDniAsync(Dni dni);
    
    /// <summary>
    /// Verifica si existe un estudiante con el email especificado
    /// </summary>
    Task<bool> ExistsByEmailAsync(Email email);
    
    /// <summary>
    /// Obtiene estudiantes con paginación y búsqueda
    /// </summary>
    Task<(List<Student> Students, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
    
    /// <summary>
    /// Obtiene estudiantes por carrera
    /// </summary>
    Task<IEnumerable<Student>> GetStudentsByCareerId(int careerId);
    
    /// <summary>
    /// Obtiene las carreras asignadas a un estudiante
    /// </summary>
    Task<IEnumerable<Career>> GetCareersByStudentIdAsync(int studentId);
}