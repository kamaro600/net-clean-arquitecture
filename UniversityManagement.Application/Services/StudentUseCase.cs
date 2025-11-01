using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Application.Mappers;
using UniversityManagement.Domain.Exceptions;
using UniversityManagement.Domain.Models;
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
        // Validaciones de negocio
        await _studentDomainService.ValidateStudentUniquenessAsync(command.Dni, command.Email);              

        // Crear entidad directamente desde command
        var student = new Student
        {
            Nombre = command.FirstName,
            Apellido = command.LastName,
            Dni = command.Dni,
            Email = command.Email,
            Telefono = command.Phone,
            FechaNacimiento = command.Birthdate,
            Direccion = command.Address
        };

        // Validar datos usando Value Objects
        student.ValidateStudentData();

        // Persistir
        var createdStudent = await _studentRepository.CreateAsync(student);

        // Enviar notificación
        await _notificationService.SendWelcomeAsync(
            createdStudent.Email,
            $"{createdStudent.Nombre} {createdStudent.Apellido}");

        return createdStudent.ToStudentData();

    }

    public async Task<StudentResponse> UpdateStudentAsync(UpdateStudentCommand command)
    {

        var existingStudent = await _studentRepository.GetByIdAsync(command.Id);
        if (existingStudent == null)
            throw new StudentNotFoundException(command.Id);            

        // Validar que el DNI no esté en uso por otro estudiante
        var studentWithDni = await _studentRepository.GetByDniAsync(command.Dni);
        if (studentWithDni != null && studentWithDni.EstudianteId != command.Id)
            throw new DuplicateStudentException("Dni", command.Dni);

        // Actualizar propiedades
        existingStudent.Nombre = command.FirstName;
        existingStudent.Apellido = command.LastName;
        existingStudent.Email = command.Email;
        existingStudent.Telefono = command.Phone;
        existingStudent.FechaNacimiento = command.BirthDate;
        existingStudent.Direccion = command.Address;
        existingStudent.Activo = command.IsActive;

        var updatedStudent = await _studentRepository.UpdateAsync(existingStudent);

        return updatedStudent.ToStudentData();

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
                student.Email,
                $"{student.Nombre} {student.Apellido}",
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

        return student.ToStudentData();

    }

    public async Task<StudentResponse> GetStudentByDniAsync(GetStudentByDniQuery query)
    {

        var student = await _studentRepository.GetByDniAsync(query.Dni);
        if (student == null)
            throw new StudentNotFoundException(query.Dni);        

        return student.ToStudentData();

    }

    public async Task<List<StudentResponse>> GetStudentsAsync(GetStudentsQuery query)
    {
        var result = await _studentRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(query.SearchTerm))
        {
            result = result
                .Where(s =>
                     s.Nombre.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                     s.Apellido.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                     s.Dni.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase) ||
                     s.Email.Contains(query.SearchTerm, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return result.ToStudentDataList();
    }

    public async Task<List<StudentResponse>> GetStudentsByCareerAsync(GetStudentsByCareerQuery query)
    {
        var result = await _studentRepository.GetStudentsByCareerId(query.CareerId);

        return result.ToStudentDataList();

    }
}