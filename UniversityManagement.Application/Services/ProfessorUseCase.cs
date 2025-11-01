using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.Mappers;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Exceptions;
using UniversityManagement.Domain.Services;
using UniversityManagement.Domain.Services.Interfaces;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Caso de uso para gestión de profesores
/// </summary>
public class ProfessorUseCase : IProfessorUseCase
{
    private readonly IProfessorRepository _professorRepository;
    private readonly IProfessorDomainService _professorDomainService;
    

    public ProfessorUseCase(IProfessorRepository professorRepository, IProfessorDomainService professorDomainService)
    {
        _professorRepository = professorRepository;
        _professorDomainService = professorDomainService;
    }

    public async Task<ProfessorResponse> CreateProfessorAsync(CreateProfessorCommand command)
    {
        await _professorDomainService.ValidateProfessorUniquenessAsync(command.Dni,command.Email);        

        // Crear el profesor
        var professor = new Professor(
            firstName: command.FirstName,
            lastName: command.LastName,
            dni: command.Dni,
            email: command.Email,
            phone: command.Phone,
            specialty: command.Specialty
        );

        var createdProfessor = await _professorRepository.CreateAsync(professor);
        return createdProfessor.ToProfessorData();
    }

    public async Task<ProfessorResponse> UpdateProfessorAsync(UpdateProfessorCommand command)
    {
        var existingProfessor = await _professorRepository.GetByIdAsync(command.Id);
        if (existingProfessor == null)
        {
            throw new ProfessorNotFoundException(command.Id);
        }

        // Validar DNI único si se está actualizando
        if (!string.IsNullOrEmpty(command.Dni) && command.Dni != existingProfessor.Dni)
        {
            var existingProfessors = await _professorRepository.GetAllAsync();
            if (existingProfessors.Any(p => p.Dni.Equals(command.Dni, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateProfessorException("Dni", command.Dni);
            }
        }

        // Validar email único si se está actualizando
        if (!string.IsNullOrEmpty(command.Email) && command.Email != existingProfessor.Email)
        {
            var existingProfessors = await _professorRepository.GetAllAsync();
            if (existingProfessors.Any(p => p.Email.Equals(command.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DuplicateProfessorException("Email", command.Email);                
            }
        }

        // Actualizar campos usando los métodos específicos de la entidad Domain
        var updatedProfessor = existingProfessor;
        
        if (!string.IsNullOrEmpty(command.FirstName) && command.FirstName != existingProfessor.FirstName)
        {
            updatedProfessor = updatedProfessor.UpdateFirstName(command.FirstName);
        }
        
        if (!string.IsNullOrEmpty(command.LastName) && command.LastName != existingProfessor.LastName)
        {
            updatedProfessor = updatedProfessor.UpdateLastName(command.LastName);
        }
        
        if (!string.IsNullOrEmpty(command.Email) && command.Email != existingProfessor.Email)
        {
            updatedProfessor = updatedProfessor.UpdateEmail(command.Email);
        }
        
        if (command.Phone != existingProfessor.Phone)
        {
            updatedProfessor = updatedProfessor.UpdatePhone(command.Phone);
        }
        
        if (command.Specialty != existingProfessor.Specialty)
        {
            updatedProfessor = updatedProfessor.UpdateSpecialty(command.Specialty);
        }
        
        if (command.IsActive.HasValue && command.IsActive.Value != existingProfessor.Activo)
        {
            updatedProfessor = command.IsActive.Value ? updatedProfessor.Activate() : updatedProfessor.Deactivate();
        }

        var resultProfessor = await _professorRepository.UpdateAsync(updatedProfessor);
        return resultProfessor.ToProfessorData();
    }

    public async Task<ProfessorResponse> GetProfessorByIdAsync(GetProfessorByIdQuery query)
    {
        var professor = await _professorRepository.GetByIdAsync(query.Id);
        if (professor == null)
        {
            throw new ProfessorNotFoundException(query.Id);
        }

        // Cargar las carreras relacionadas con el profesor
        var careers = await _professorRepository.GetCareersByProfessorIdAsync(query.Id);

        return professor.ToProfessorData(careers);
    }

    public async Task<List<ProfessorResponse>> GetProfessorsAsync(GetProfessorsQuery query)
    {
        var result = await _professorRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            result = result.Where(p =>
                     (!string.IsNullOrEmpty(p.FirstName) && p.FirstName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                     (!string.IsNullOrEmpty(p.LastName) && p.LastName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                     (!string.IsNullOrEmpty(p.Dni) && p.Dni.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                     (!string.IsNullOrEmpty(p.Email) && p.Email.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
                )
                .ToList();
        }

        return result.ToProfessorDataList();
    }

    public async Task<DeletionResponse> DeleteProfessorAsync(DeleteProfessorCommand command)
    {

        var professor = await _professorRepository.GetByIdAsync(command.Id);
        if (professor == null)
        {
            return DeletionResponse.NotFound($"Profesor con ID {command.Id} no encontrado");
        }

        var deleted = await _professorRepository.DeleteAsync(command.Id);
        return deleted
            ? DeletionResponse.Success($"Profesor con ID {command.Id} eliminado exitosamente")
            : DeletionResponse.Failure("No se pudo eliminar el profesor");
    }
}