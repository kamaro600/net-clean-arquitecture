using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.DTOs.Queries;

namespace UniversityManagement.Application.Ports.In;

/// <summary>
/// Puerto de entrada para casos de uso de facultades
/// </summary>
public interface IFacultyUseCase
{
    // Commands (Escritura)
    Task<FacultyResponse> CreateFacultyAsync(CreateFacultyCommand command);
    Task<FacultyResponse> UpdateFacultyAsync(UpdateFacultyCommand command);
    Task<DeletionResponse> DeleteFacultyAsync(DeleteFacultyCommand command);
    
    // Queries (Lectura)
    Task<FacultyResponse> GetFacultyByIdAsync(GetFacultyByIdQuery query);
    Task<List<FacultyResponse>> GetFacultiesByNameAsync(GetFacultiesQuery query);
}