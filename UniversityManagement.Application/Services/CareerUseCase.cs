using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.Mappers;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Exceptions;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Caso de uso para gestión de carreras
/// Orquesta las operaciones de negocio relacionadas con carreras
/// </summary>
public class CareerUseCase : ICareerUseCase
{
    private readonly ICareerRepository _careerRepository;
    private readonly IFacultyRepository _facultyRepository;

    public CareerUseCase(
        ICareerRepository careerRepository,
        IFacultyRepository facultyRepository)
    {
        _careerRepository = careerRepository;
        _facultyRepository = facultyRepository;
    }

    public async Task<CareerResponse> CreateCareerAsync(CreateCareerCommand command)
    {
        // Validar que la facultad exista
        if (await _facultyRepository.GetByIdAsync(command.FacultyId) is null)
        {
            throw new FacultyNotFoundException(command.FacultyId);
        }
        // Validar que no exista una carrera con el mismo nombre
        if (await _careerRepository.ExistsByNameAsync(command.Name))
        {
            throw new DuplicateCareerException("Nombre", command.Name);
        }
        // Crear la carrera
        var career = new Career
        {
            Nombre = command.Name,
            Descripcion = command.Description,
            FacultadId = command.FacultyId,
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        };

        var createdCareer = await _careerRepository.CreateAsync(career);
        return createdCareer.ToCareerData();
    }

    public async Task<CareerResponse> UpdateCareerAsync(UpdateCareerCommand command)
    {
        var existingCareer = await _careerRepository.GetByIdAsync(command.Id);
        if (existingCareer == null)
        {
            throw new CareerNotFoundException(command.Id);            
        }

        // Validar que la facultad exista si se está actualizando
        if (command.FacultyId.HasValue && await _facultyRepository.GetByIdAsync(command.FacultyId.Value) is not null)
        {
            throw new FacultyNotFoundException(command.FacultyId.Value);
        }

        // Validar nombre único si se está actualizando
        if (!string.IsNullOrEmpty(command.Name) && command.Name != existingCareer.Nombre)
        {
            if (await _careerRepository.ExistsByNameAsync(command.Name))
            {
                throw new DuplicateCareerException("Nombre", command.Name);
            }
        }

        // Actualizar campos
        existingCareer.Nombre = command.Name ?? existingCareer.Nombre;
        existingCareer.Descripcion = command.Description ?? existingCareer.Descripcion;
        existingCareer.FacultadId = command.FacultyId ?? existingCareer.FacultadId;
        if (command.IsActive.HasValue)
            existingCareer.Activo = command.IsActive.Value;

        var updatedCareer = await _careerRepository.UpdateAsync(existingCareer);
        return updatedCareer.ToCareerData();

    }

    public async Task<CareerResponse> GetCareerByIdAsync(GetCareerByIdQuery query)
    {
        var career = await _careerRepository.GetByIdAsync(query.Id);
        return career == null
            ? throw new CareerNotFoundException(query.Id)
            : career.ToCareerData();
    }

    public async Task<List<CareerResponse>> GetCareersByNameAsync(GetCareersQuery query)
    {
        if (string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            throw new ArgumentException("El término de búsqueda no puede ser nulo o vacío.", nameof(query.SearchTerm));
        }

        var result = await _careerRepository.GetAllAsync();

        result = result
            .Where(c => c.Nombre.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
            .ToList();

        return result.ToCareerDataList();
    }

    public async Task<List<CareerResponse>> GetCareersByFacultyAsync(GetCareersByFacultyQuery query)
    {
        // Validar que la facultad exista
        if (await _facultyRepository.GetByIdAsync(query.FacultyId) is null)
        {
            throw new FacultyNotFoundException(query.FacultyId);
        }

        var paged = await _careerRepository.GetByFacultyIdAsync(query.FacultyId);

        return paged.ToCareerDataList();

    }

    public async Task<DeletionResponse> DeleteCareerAsync(DeleteCareerCommand command)
    {

        var career = await _careerRepository.GetByIdAsync(command.Id);
        if (career == null)
        {
            return DeletionResponse.NotFound("Carrera no encontrada.");
        }

        var deleted = await _careerRepository.DeleteAsync(command.Id);
        return deleted
            ? DeletionResponse.Success("Carrera eliminada exitosamente.")
            : DeletionResponse.Failure("No se pudo eliminar la carrera.");


    }
}