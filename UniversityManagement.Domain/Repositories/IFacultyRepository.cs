using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

/// <summary>
/// Interfaz del repositorio de facultades - Trabaja con entidades de dominio puras
/// </summary>
public interface IFacultyRepository
{
    /// <summary>
    /// Obtiene todas las facultades activas
    /// </summary>
    Task<IEnumerable<Faculty>> GetAllAsync();
    
    /// <summary>
    /// Obtiene una facultad por su ID
    /// </summary>
    Task<Faculty?> GetByIdAsync(int id);
    
    /// <summary>
    /// Obtiene una facultad por su nombre
    /// </summary>
    Task<Faculty?> GetByNameAsync(string name);
    
    /// <summary>
    /// Crea una nueva facultad
    /// </summary>
    Task<Faculty> CreateAsync(Faculty faculty);
    
    /// <summary>
    /// Actualiza una facultad existente
    /// </summary>
    Task<Faculty> UpdateAsync(Faculty faculty);
    
    /// <summary>
    /// Elimina una facultad (soft delete)
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Verifica si existe una facultad con el nombre especificado
    /// </summary>
    Task<bool> ExistsByNameAsync(string name);
    
    /// <summary>
    /// Obtiene facultades con paginación y búsqueda
    /// </summary>
    Task<(List<Faculty> Faculties, int TotalCount)> GetPagedAsync(int page, int pageSize, string? searchTerm = null);
}