using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Application.Ports.In;

public interface IStudentUseCase
{
    // Commands (Escritura)
    Task<StudentResponse> CreateStudentAsync(CreateStudentCommand command);
    Task<StudentResponse> UpdateStudentAsync(UpdateStudentCommand command);
    Task<DeletionResponse> DeleteStudentAsync(DeleteStudentCommand command);
    
    // Queries (Lectura)
    Task<StudentResponse> GetStudentByIdAsync(GetStudentByIdQuery query);
    Task<StudentResponse> GetStudentByDniAsync(GetStudentByDniQuery query);
}

