namespace UniversityManagement.Domain.Services.Interfaces;

public interface IStudentDomainService
{
    Task ValidateStudentUniquenessAsync(string dni, string email, int? excludeStudentId = null);
    Task<bool> CanEnrollInCareerAsync(int studentId, int careerId);
    Task EnrollStudentInCareerAsync(int studentId, int careerId);
}