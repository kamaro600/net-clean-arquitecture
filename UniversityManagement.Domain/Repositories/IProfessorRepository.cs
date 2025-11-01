using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

/// <summary>
/// Interfaz del repositorio de profesores - Trabaja con entidades de dominio puras
/// </summary>
public interface IProfessorRepository
{
    /// <summary>
    /// Obtiene todos los profesores activos
    /// </summary>
    Task<IEnumerable<Professor>> GetAllAsync();
    
    /// <summary>
    /// Obtiene un profesor por su ID
    /// </summary>
    Task<Professor?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtiene un profesor por su DNI
    /// </summary>
    Task<Professor?> GetByDniAsync(string dni);
    
    /// <summary>
    /// Obtiene un profesor por su email
    /// </summary>
    Task<Professor?> GetByEmailAsync(string email);
    
    /// <summary>
    /// Crea un nuevo profesor
    /// </summary>
    Task<Professor> CreateAsync(Professor professor);
    
    /// <summary>
    /// Actualiza un profesor existente
    /// </summary>
    Task<Professor> UpdateAsync(Professor professor);
    
    /// <summary>
    /// Elimina un profesor (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Verifica si existe un profesor con el DNI especificado
    /// </summary>
    Task<bool> ExistsByDniAsync(string dni);
    
    /// <summary>
    /// Verifica si existe un profesor con el email especificado
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email);
    
    /// <summary>
    /// Obtiene profesores por carrera
    /// </summary>
    Task<IEnumerable<Professor>> GetProfessorsByCareerId(int careerId);
    
    /// <summary>
    /// Obtiene las carreras asignadas a un profesor
    /// </summary>
    Task<IEnumerable<Career>> GetCareersByProfessorIdAsync(int professorId);
    
    /// <summary>
    /// Obtiene profesores con paginación y búsqueda
    /// </summary>
    Task<(List<Professor> Professors, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
}