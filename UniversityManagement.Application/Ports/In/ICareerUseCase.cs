using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.DTOs.Queries;

namespace UniversityManagement.Application.Ports.In;

public interface ICareerUseCase
{
    // Commands (Escritura)
    Task<CareerResponse> CreateCareerAsync(CreateCareerCommand command);
    Task<CareerResponse> UpdateCareerAsync(UpdateCareerCommand command);
    Task<DeletionResponse> DeleteCareerAsync(DeleteCareerCommand command);
    
    // Queries (Lectura)
    Task<CareerResponse> GetCareerByIdAsync(GetCareerByIdQuery query);
    Task<List<CareerResponse>>  GetCareersByNameAsync(GetCareersQuery query);
    Task<List<CareerResponse>> GetCareersByFacultyAsync(GetCareersByFacultyQuery query);
    

}