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
        var professor = new Professor
        {
            FirstName = command.FirstName,
            LastName = command.LastName,
            Dni = command.Dni,
            Email = command.Email,
            Phone = command.Phone,
            Specialty = command.Specialty,
            Activo = true,
            FechaRegistro = DateTime.UtcNow
        };

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
            if (await _professorRepository.ExistsByDniAsync(command.Dni))
            {
                throw new DuplicateProfessorException("Dni", command.Dni);
            }
        }

        // Validar email único si se está actualizando
        if (!string.IsNullOrEmpty(command.Email) && command.Email != existingProfessor.Email)
        {
            if (await _professorRepository.ExistsByEmailAsync(command.Email))
            {
                throw new DuplicateProfessorException("Email", command.Email);                
            }
        }

        // Actualizar campos
        existingProfessor.FirstName = command.FirstName ?? existingProfessor.FirstName;
        existingProfessor.LastName = command.LastName ?? existingProfessor.LastName;
        existingProfessor.Dni = command.Dni ?? existingProfessor.Dni;
        existingProfessor.Email = command.Email ?? existingProfessor.Email;
        existingProfessor.Phone = command.Phone ?? existingProfessor.Phone;
        existingProfessor.Specialty = command.Specialty ?? existingProfessor.Specialty;
        if (command.IsActive.HasValue)
            existingProfessor.Activo = command.IsActive.Value;

        var updatedProfessor = await _professorRepository.UpdateAsync(existingProfessor);
        return updatedProfessor.ToProfessorData();

    }

    public async Task<ProfessorResponse> GetProfessorByIdAsync(GetProfessorByIdQuery query)
    {

        var professor = await _professorRepository.GetByIdAsync(query.Id);
        return professor == null
            ? throw new ProfessorNotFoundException(query.Id)
            : professor.ToProfessorData();
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