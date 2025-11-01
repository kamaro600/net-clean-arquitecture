using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

/// <summary>
/// Interfaz del repositorio de carreras - Trabaja con entidades de dominio puras
/// </summary>
public interface ICareerRepository
{
    /// <summary>
    /// Obtiene todas las carreras activas
    /// </summary>
    Task<IEnumerable<Career>> GetAllAsync();
    
    /// <summary>
    /// Obtiene una carrera por su ID
    /// </summary>
    Task<Career?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtiene una carrera por su nombre
    /// </summary>
    Task<Career?> GetByNameAsync(string name);
    
    /// <summary>
    /// Obtiene carreras por facultad
    /// </summary>
    Task<IEnumerable<Career>> GetByFacultyIdAsync(int facultyId);
    
    /// <summary>
    /// Crea una nueva carrera
    /// </summary>
    Task<Career> CreateAsync(Career career);
    
    /// <summary>
    /// Actualiza una carrera existente
    /// </summary>
    Task<Career> UpdateAsync(Career career);
    
    /// <summary>
    /// Elimina una carrera (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Verifica si existe una carrera con el nombre especificado en una facultad
    /// </summary>
    Task<bool> ExistsByNameInFacultyAsync(string name, int facultyId);
    
    /// <summary>
    /// Obtiene carreras con paginación y búsqueda
    /// </summary>
    Task<(List<Career> Careers, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
}