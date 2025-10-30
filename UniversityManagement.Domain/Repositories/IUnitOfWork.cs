namespace UniversityManagement.Domain.Repositories;

public interface IUnitOfWork
{
    IStudentRepository Students { get; }
    IProfessorRepository Professors { get; }
    IFacultyRepository Faculties { get; }
    ICareerRepository Careers { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}