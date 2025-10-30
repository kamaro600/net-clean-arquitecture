namespace UniversityManagement.Domain.Services.Interfaces;

public interface IProfessorDomainService
{
    Task ValidateProfessorUniquenessAsync(string dni, string email, int? excludeProfessorId = null);
    Task<bool> CanAssignToCareerAsync(int professorId, int careerId);
}