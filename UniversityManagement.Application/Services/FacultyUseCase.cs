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
    private readonly ICareerRepository _careerRepository;

    public FacultyUseCase(IFacultyRepository facultyRepository, ICareerRepository careerRepository)
    {
        _facultyRepository = facultyRepository;
        _careerRepository = careerRepository;
    }

    public async Task<FacultyResponse> CreateFacultyAsync(CreateFacultyCommand command)
    {
        // Validar que no exista una facultad con el mismo nombre
        var existingFaculties = await _facultyRepository.GetAllAsync();
        if (existingFaculties.Any(f => f.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateFacultyException("Nombre", command.Name);
        }

        // Crear la facultad
        var faculty = new Faculty(
            name: command.Name,
            description: command.Description
        );

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
            var existingFaculties = await _facultyRepository.GetAllAsync();
            if (existingFaculties.Any(f => f.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateFacultyException("Nombre", command.Name);                
            }
        }

        // Actualizar campos usando los métodos específicos de la entidad Domain
        var updatedFaculty = existingFaculty;
        
        if (!string.IsNullOrEmpty(command.Name) && command.Name != existingFaculty.Name)
        {
            updatedFaculty = updatedFaculty.UpdateName(command.Name);
        }
        
        if (command.Description != existingFaculty.Description)
        {
            updatedFaculty = updatedFaculty.UpdateDescription(command.Description);
        }
        
        if (command.IsActive.HasValue && command.IsActive.Value != existingFaculty.Activo)
        {
            updatedFaculty = command.IsActive.Value ? updatedFaculty.Activate() : updatedFaculty.Deactivate();
        }

        var resultFaculty = await _facultyRepository.UpdateAsync(updatedFaculty);
        return resultFaculty.ToFacultyData();
    }

    public async Task<FacultyResponse> GetFacultyByIdAsync(GetFacultyByIdQuery query)
    {
        var faculty = await _facultyRepository.GetByIdAsync(query.Id);
        if (faculty == null)
        {
            throw new FacultyNotFoundException(query.Id);
        }

        // Cargar las carreras relacionadas con la facultad
        var careers = await _careerRepository.GetByFacultyIdAsync(query.Id);

        return faculty.ToFacultyData(careers);
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

        // Para obtener facultades con sus carreras, cargar las carreras para cada facultad
        var facultiesWithCareers = new List<FacultyResponse>();
        
        foreach (var faculty in result)
        {
            var careers = await _careerRepository.GetByFacultyIdAsync(faculty.FacultyId);
            facultiesWithCareers.Add(faculty.ToFacultyData(careers));
        }

        return facultiesWithCareers;
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