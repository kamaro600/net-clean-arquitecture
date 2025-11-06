using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Models.ValueObjects;
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
        // Crear Value Objects para validación
        var dniVO = new Dni(dni);
        var emailVO = new Email(email);

        var existingStudentByDni = await _studentRepository.GetByDniAsync(dniVO);
        if (existingStudentByDni != null && existingStudentByDni.Id != excludeStudentId)
        {
            throw new DuplicateStudentException("DNI", dni);
        }

        var existingStudentByEmail = await _studentRepository.GetByEmailAsync(emailVO);
        if (existingStudentByEmail != null && existingStudentByEmail.Id != excludeStudentId)
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

        // Lógica de negocio usando entidad de dominio
        return student.CanEnrollInCareer(DateTime.Now) && student.IsActive;
    }

    public async Task EnrollStudentInCareerAsync(int studentId, int careerId)
    {
        if (!await CanEnrollInCareerAsync(studentId, careerId))
            throw new InvalidOperationException("El estudiante no puede inscribirse en esta carrera");
    }

    public async Task<StudentCareer> EnrollStudentInCareerAsync(Student student, Career career)
    {
        if (student == null)
            throw new ArgumentNullException(nameof(student));
        
        if (career == null)
            throw new ArgumentNullException(nameof(career));

        if (!student.CanEnrollInCareer(career))
            throw new InvalidOperationException("El estudiante no puede inscribirse en esta carrera");

        var enrollment = new StudentCareer(student.Id, career.CareerId, DateTime.UtcNow, true, student, career);
        
        // Enviar notificación de matrícula
        await NotifyEnrollmentAsync(student, career);
        
        return enrollment;
    }

    public async Task NotifyEnrollmentAsync(Student student, Career career)
    {
        // Aquí iría la lógica de notificación (email, SMS, etc.)
        // Por ahora, solo log conceptual
        await Task.Delay(10); // Simular operación async
        
        // En una implementación real, se usarían los ports de Application para envío de notificaciones
        Console.WriteLine($"Estudiante {student.FullName.FirstName} {student.FullName.LastName} matriculado en {career.Name}");
    }

    public async Task NotifyUnenrollmentAsync(Student student, Career career)
    {
        // Aquí iría la lógica de notificación de desmatrícula
        await Task.Delay(10); // Simular operación async
        
        Console.WriteLine($"Estudiante {student.FullName.FirstName} {student.FullName.LastName} desmatriculado de {career.Name}");
    }
}