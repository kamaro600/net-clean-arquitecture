using UniversityManagement.Domain.Models;

namespace UniversityManagement.Domain.Repositories;

public interface IStudentRepository
{
    Task<IEnumerable<Student>> GetAllAsync();
    Task<Student?> GetByIdAsync(int id);
    Task<Student?> GetByDniAsync(string dni);
    Task<Student?> GetByEmailAsync(string email);
    Task<Student> CreateAsync(Student student);
    Task<Student> UpdateAsync(Student student);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsByDniAsync(string dni);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<Student>> GetStudentsByCareerId(int careerId);
}