using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

public interface ICareerRepository
{
    Task<IEnumerable<Career>> GetAllAsync();
    Task<Career?> GetByIdAsync(int id);
    Task<Career?> GetByNameAsync(string name);
    Task<IEnumerable<Career>> GetByFacultyIdAsync(int facultyId);
    Task<Career> CreateAsync(Career career);
    Task<Career> UpdateAsync(Career career);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByNameAsync(string name);
}