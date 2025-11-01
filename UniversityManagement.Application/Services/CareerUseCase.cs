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
/// </summary>
public class CareerUseCase : ICareerUseCase
{
    private readonly ICareerRepository _careerRepository;
    private readonly IFacultyRepository _facultyRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly IProfessorRepository _professorRepository;

    public CareerUseCase(
        ICareerRepository careerRepository,
        IFacultyRepository facultyRepository,
        IStudentRepository studentRepository,
        IProfessorRepository professorRepository)
    {
        _careerRepository = careerRepository;
        _facultyRepository = facultyRepository;
        _studentRepository = studentRepository;
        _professorRepository = professorRepository;
    }

    public async Task<CareerResponse> CreateCareerAsync(CreateCareerCommand command)
    {
        // Validar que la facultad exista
        if (await _facultyRepository.GetByIdAsync(command.FacultyId) is null)
        {
            throw new FacultyNotFoundException(command.FacultyId);
        }

        // Validar que no exista una carrera con el mismo nombre
        var existingCareers = await _careerRepository.GetAllAsync();
        if (existingCareers.Any(c => c.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
        {
            throw new DuplicateCareerException("Nombre", command.Name);
        }

        // Crear la carrera
        var career = new Career(
            facultyId: command.FacultyId,
            name: command.Name,
            semesterDuration: 8, // Valor por defecto
            description: command.Description
        );

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
        if (command.FacultyId.HasValue && await _facultyRepository.GetByIdAsync(command.FacultyId.Value) is null)
        {
            throw new FacultyNotFoundException(command.FacultyId.Value);
        }

        // Validar nombre único si se está actualizando
        if (!string.IsNullOrEmpty(command.Name) && command.Name != existingCareer.Name)
        {
            var existingCareers = await _careerRepository.GetAllAsync();
            if (existingCareers.Any(c => c.Name.Equals(command.Name, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateCareerException("Nombre", command.Name);
            }
        }

        // Actualizar campos usando los métodos específicos de la entidad Domain
        var updatedCareer = existingCareer;

        if (!string.IsNullOrEmpty(command.Name) && command.Name != existingCareer.Name)
        {
            updatedCareer = updatedCareer.UpdateName(command.Name);
        }

        if (command.Description != existingCareer.Description)
        {
            updatedCareer = updatedCareer.UpdateDescription(command.Description);
        }

        if (command.FacultyId.HasValue && command.FacultyId.Value != existingCareer.FacultyId)
        {
            updatedCareer = updatedCareer.TransferToFaculty(command.FacultyId.Value);
        }

        if (command.IsActive.HasValue && command.IsActive.Value != existingCareer.Activo)
        {
            updatedCareer = command.IsActive.Value ? updatedCareer.Activate() : updatedCareer.Deactivate();
        }

        var resultCareer = await _careerRepository.UpdateAsync(updatedCareer);
        return resultCareer.ToCareerData();
    }

    public async Task<CareerResponse> GetCareerByIdAsync(GetCareerByIdQuery query)
    {
        var career = await _careerRepository.GetByIdAsync(query.Id);
        if (career == null)
        {
            throw new CareerNotFoundException(query.Id);
        }

        // Cargar datos relacionados
        var faculty = await _facultyRepository.GetByIdAsync(career.FacultyId);
        var students = await _studentRepository.GetStudentsByCareerId(career.CareerId);
        var professors = await _professorRepository.GetProfessorsByCareerId(career.CareerId);

        return career.ToCareerData(faculty, students, professors);
    }

    public async Task<List<CareerResponse>> GetCareersByNameAsync(GetCareersQuery query)
    {

        var result = await _careerRepository.GetAllAsync();

        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            result = result
         .Where(c => c.Name.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
         .ToList();
        }

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