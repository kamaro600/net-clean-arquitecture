using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.Mappers;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Exceptions;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Caso de uso para gestión de facultades
/// </summary>
public class FacultyUseCase : IFacultyUseCase
{
    private readonly IFacultyRepository _facultyRepository;

    public FacultyUseCase(IFacultyRepository facultyRepository)
    {
        _facultyRepository = facultyRepository;
    }

    public async Task<FacultyResponse> CreateFacultyAsync(CreateFacultyCommand command)
    {
        // Validar que no exista una facultad con el mismo nombre
        if (await _facultyRepository.ExistsByNameAsync(command.Name))
        {
            throw new DuplicateFacultyException("Nombre", command.Name);
        }

        // Crear la facultad
        var faculty = new Faculty
        {
            Name = command.Name,
            Description = command.Description,
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        };

        var createdFaculty = await _facultyRepository.CreateAsync(faculty);
        return createdFaculty.ToFacultyData();

    }

    public async Task<FacultyResponse> UpdateFacultyAsync(UpdateFacultyCommand command)
    {

        var existingFaculty = await _facultyRepository.GetByIdAsync(command.Id);
        if (existingFaculty == null)
        {
            throw new FacultyNotFoundException(command.Id);
        }

        // Validar nombre único si se está actualizando
        if (!string.IsNullOrEmpty(command.Name) && command.Name != existingFaculty.Name)
        {
            if (await _facultyRepository.ExistsByNameAsync(command.Name))
            {
                throw new DuplicateFacultyException("Nombre", command.Name);                
            }
        }

        // Actualizar campos
        existingFaculty.Name = command.Name ?? existingFaculty.Name;
        existingFaculty.Description = command.Description ?? existingFaculty.Description;
        if (command.IsActive.HasValue)
            existingFaculty.Activo = command.IsActive.Value;

        var updatedFaculty = await _facultyRepository.UpdateAsync(existingFaculty);
        return updatedFaculty.ToFacultyData();

    }

    public async Task<FacultyResponse> GetFacultyByIdAsync(GetFacultyByIdQuery query)
    {

        var faculty = await _facultyRepository.GetByIdAsync(query.Id);
        return faculty == null
            ? throw new FacultyNotFoundException(query.Id)
            : faculty.ToFacultyData();

    }

    public async Task<List<FacultyResponse>> GetFacultiesByNameAsync(GetFacultiesQuery query)
    {
        var result = await _facultyRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            result = result
                .Where(f => f.Name.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result.ToFacultyDataList();
    }

    public async Task<DeletionResponse> DeleteFacultyAsync(DeleteFacultyCommand command)
    {

        var faculty = await _facultyRepository.GetByIdAsync(command.Id);
        if (faculty == null)
        {
            return DeletionResponse.NotFound($"Facultad con ID {command.Id} no encontrada");
        }

        var deleted = await _facultyRepository.DeleteAsync(command.Id);
        return deleted
            ? DeletionResponse.Success($"Facultad con ID {command.Id} eliminada exitosamente")
            : DeletionResponse.Failure("No se pudo eliminar la facultad");

    }
}