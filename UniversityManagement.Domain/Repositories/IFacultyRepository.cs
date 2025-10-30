using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

public interface IFacultyRepository
{
    Task<IEnumerable<Faculty>> GetAllAsync();
    Task<Faculty?> GetByIdAsync(int id);
    Task<Faculty?> GetByNameAsync(string name);
    Task<Faculty> CreateAsync(Faculty faculty);
    Task<Faculty> UpdateAsync(Faculty faculty);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}