using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Exceptions;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Services.Interfaces;

namespace UniversityManagement.Domain.Services;

public class ProfessorDomainService : IProfessorDomainService
{
    private readonly IProfessorRepository _professorRepository;
    private readonly ICareerRepository _careerRepository;

    public ProfessorDomainService(IProfessorRepository professorRepository, ICareerRepository careerRepository)
    {
        _professorRepository = professorRepository;
        _careerRepository = careerRepository;
    }

    public async Task ValidateProfessorUniquenessAsync(string dni, string email, int? excludeProfessorId = null)
    {
        var existingProfessorByDni = await _professorRepository.GetByDniAsync(dni);
        if (existingProfessorByDni != null && existingProfessorByDni.ProfesorId != excludeProfessorId)
        {
            throw new DuplicateProfessorException("DNI", dni);
        }

        var existingProfessorByEmail = await _professorRepository.GetByEmailAsync(email);
        if (existingProfessorByEmail != null && existingProfessorByEmail.ProfesorId != excludeProfessorId)
        {
            throw new DuplicateProfessorException("Email", email);
        }
    }

    public async Task<bool> CanAssignToCareerAsync(int professorId, int careerId)
    {
        var professor = await _professorRepository.GetByIdAsync(professorId);
        if (professor == null)
            return false;

        var career = await _careerRepository.GetByIdAsync(careerId);
        if (career == null)
            return false;

        // Verificar si ya está asignado a esa carrera
        return !professor.ProfessorCareers.Any(pc => pc.CarreraId == careerId && pc.Activo);
    }
}