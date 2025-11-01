using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Application.Mappers;
using UniversityManagement.Domain.Exceptions;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Models.ValueObjects;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Services.Interfaces;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Implementación de casos de uso de estudiantes
/// </summary>
public class StudentUseCase : IStudentUseCase
{
    private readonly IStudentRepository _studentRepository;
    private readonly IEmailNotificationPort _notificationService;
    private readonly IStudentDomainService _studentDomainService;

    public StudentUseCase(
        IStudentRepository studentRepository,
        IEmailNotificationPort notificationService, IStudentDomainService studentDomainService)
    {
        _studentRepository = studentRepository;
        _notificationService = notificationService;
        _studentDomainService = studentDomainService;
    }

    public async Task<StudentResponse> CreateStudentAsync(CreateStudentCommand command)
    {
        // Crear Value Objects desde el command con validación automática
        var fullName = new FullName(command.FirstName, command.LastName);
        var dni = new Dni(command.Dni);
        var email = new Email(command.Email);
        
        // Value Objects opcionales
        Phone? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            phone = new Phone(command.Phone);
        }
        
        Address? address = null;
        if (!string.IsNullOrWhiteSpace(command.Address))
        {
            address = new Address(command.Address);
        }

        // Validaciones de negocio usando Value Objects
        await _studentDomainService.ValidateStudentUniquenessAsync(dni.Value, email.Value);

        // Crear entidad de dominio
        var studentDomain = new Student(
            fullName: fullName,
            dni: dni,
            email: email,
            birthdate: command.Birthdate,
            phone: phone,
            address: address
        );

        // Persistir usando el repositorio
        var createdStudent = await _studentRepository.CreateAsync(studentDomain);

        // Enviar notificación
        await _notificationService.SendWelcomeAsync(
            createdStudent.Email.Value,
            createdStudent.GetFormattedFullName());

        return StudentMapper.ToResponse(createdStudent);
    }

    public async Task<StudentResponse> UpdateStudentAsync(UpdateStudentCommand command)
    {
        var existingStudent = await _studentRepository.GetByIdAsync(command.Id);
        if (existingStudent == null)
            throw new StudentNotFoundException(command.Id);

        // Crear nuevos Value Objects con validación automática
        var fullName = new FullName(command.FirstName, command.LastName);
        var dni = new Dni(command.Dni);
        var email = new Email(command.Email);
        
        // Validar que el DNI no esté en uso por otro estudiante
        if (dni != existingStudent.Dni)
        {
            var studentWithDni = await _studentRepository.GetByDniAsync(dni);
            if (studentWithDni != null && studentWithDni.Id != command.Id)
                throw new DuplicateStudentException("Dni", command.Dni);
        }

        // Value Objects opcionales
        Phone? phone = null;
        if (!string.IsNullOrWhiteSpace(command.Phone))
        {
            phone = new Phone(command.Phone);
        }
        
        Address? address = null;
        if (!string.IsNullOrWhiteSpace(command.Address))
        {
            address = new Address(command.Address);
        }

        // Crear nueva instancia inmutable con los cambios
        var updatedStudentDomain = new Student(
            id: existingStudent.Id,
            fullName: fullName,
            dni: dni,
            email: email,
            birthdate: command.BirthDate,
            phone: phone,
            address: address,
            registrationDate: existingStudent.RegistrationDate,
            isActive: command.IsActive
        );

        var updatedStudent = await _studentRepository.UpdateAsync(updatedStudentDomain);

        return StudentMapper.ToResponse(updatedStudent);
    }

    public async Task<DeletionResponse> DeleteStudentAsync(DeleteStudentCommand command)
    {
        var student = await _studentRepository.GetByIdAsync(command.Id);
        if (student == null)
            return DeletionResponse.NotFound($"No se encontró el estudiante con ID: {command.Id}");

        var result = await _studentRepository.DeleteAsync(command.Id);

        if (result)
        {
            await _notificationService.SendStudentUpdateNotificationAsync(
                student.Email.Value,
                student.GetFormattedFullName(),
                new List<string> { "Eliminación de cuenta" });

            return DeletionResponse.Success($"Estudiante con ID {command.Id} eliminado exitosamente");
        }

        return DeletionResponse.Failure("Error al eliminar estudiante");
    }

    public async Task<StudentResponse> GetStudentByIdAsync(GetStudentByIdQuery query)
    {
        var student = await _studentRepository.GetByIdAsync(query.StudentId);
        if (student == null)
            throw new StudentNotFoundException(query.StudentId);

        // Cargar las carreras relacionadas con el estudiante
        var careers = await _studentRepository.GetCareersByStudentIdAsync(query.StudentId);

        return StudentMapper.ToResponse(student, careers);
    }

    public async Task<StudentResponse> GetStudentByDniAsync(GetStudentByDniQuery query)
    {
        var dni = new Dni(query.Dni); // Validación automática del DNI
        var student = await _studentRepository.GetByDniAsync(dni);
        if (student == null)
            throw new StudentNotFoundException(query.Dni);

        return StudentMapper.ToResponse(student);
    }

    public async Task<List<StudentResponse>> GetStudentsAsync(GetStudentsQuery query)
    {
        var result = await _studentRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            result = result
                .Where(s =>
                     s.FullName.FirstName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                     s.FullName.LastName.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                     s.Dni.Value.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                     s.Email.Value.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result.Select(StudentMapper.ToResponse).ToList();
    }

    public async Task<List<StudentResponse>> GetStudentsByCareerAsync(GetStudentsByCareerQuery query)
    {
        var result = await _studentRepository.GetStudentsByCareerId(query.CareerId);
        return result.Select(StudentMapper.ToResponse).ToList();
    }
}