using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.DTOs.Queries;

namespace UniversityManagement.Application.Ports.In;

public interface IProfessorUseCase
{
    // Commands (Escritura)
    Task<ProfessorResponse> CreateProfessorAsync(CreateProfessorCommand command);
    Task<ProfessorResponse> UpdateProfessorAsync(UpdateProfessorCommand command);
    Task<DeletionResponse> DeleteProfessorAsync(DeleteProfessorCommand command);
    
    // Queries (Lectura)
    Task<ProfessorResponse> GetProfessorByIdAsync(GetProfessorByIdQuery query);
    Task<List<ProfessorResponse>> GetProfessorsAsync(GetProfessorsQuery query);
}