using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

public interface IProfessorRepository
{
    Task<IEnumerable<Professor>> GetAllAsync();
    Task<Professor?> GetByIdAsync(int id);
    Task<Professor?> GetByDniAsync(string dni);
    Task<Professor?> GetByEmailAsync(string email);
    Task<Professor> CreateAsync(Professor professor);
    Task<Professor> UpdateAsync(Professor professor);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByDniAsync(string dni);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<Professor>> GetProfessorsByCareerId(int careerId);
}