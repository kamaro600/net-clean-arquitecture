using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Exceptions;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Services.Interfaces;

namespace UniversityManagement.Domain.Services;

public class StudentDomainService : IStudentDomainService
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICareerRepository _careerRepository;

    public StudentDomainService(IStudentRepository studentRepository, ICareerRepository careerRepository)
    {
        _studentRepository = studentRepository;
        _careerRepository = careerRepository;
    }

    public async Task ValidateStudentUniquenessAsync(string dni, string email, int? excludeStudentId = null)
    {
        var existingStudentByDni = await _studentRepository.GetByDniAsync(dni);
        if (existingStudentByDni != null && existingStudentByDni.EstudianteId != excludeStudentId)
        {
            throw new DuplicateStudentException("DNI", dni);
        }

        var existingStudentByEmail = await _studentRepository.GetByEmailAsync(email);
        if (existingStudentByEmail != null && existingStudentByEmail.EstudianteId != excludeStudentId)
        {
            throw new DuplicateStudentException("Email", email);
        }
    }

    public async Task<bool> CanEnrollInCareerAsync(int studentId, int careerId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        if (student == null)
            throw new StudentNotFoundException(studentId);

        var career = await _careerRepository.GetByIdAsync(careerId);
        if (career == null)
            return false;

        // Verificar si ya está inscrito en esa carrera
        return !student.StudentCareers.Any(sc => sc.CareerId == careerId && sc.IsActive);
    }

    public async Task EnrollStudentInCareerAsync(int studentId, int careerId)
    {
        if (!await CanEnrollInCareerAsync(studentId, careerId))
            throw new InvalidOperationException("El estudiante no puede inscribirse en esta carrera");
    }
}