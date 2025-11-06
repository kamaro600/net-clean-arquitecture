using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.DTOs.Messages;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Services.Interfaces;

namespace UniversityManagement.Application.Services;

/// <summary>
/// Implementación de casos de uso para matrícula de estudiantes
/// </summary>
public class EnrollmentUseCase : IEnrollmentUseCase
{
    private readonly IStudentRepository _studentRepository;
    private readonly ICareerRepository _careerRepository;
    private readonly IStudentCareerRepository _studentCareerRepository;
    private readonly IStudentDomainService _studentDomainService;
    private readonly IMessagePublisherPort _messagePublisher;

    public EnrollmentUseCase(
        IStudentRepository studentRepository,
        ICareerRepository careerRepository,
        IStudentCareerRepository studentCareerRepository,
        IStudentDomainService studentDomainService,
        IMessagePublisherPort messagePublisher)
    {
        _studentRepository = studentRepository;
        _careerRepository = careerRepository;
        _studentCareerRepository = studentCareerRepository;
        _studentDomainService = studentDomainService;
        _messagePublisher = messagePublisher;
    }

    public async Task<EnrollmentResponse> EnrollStudentInCareerAsync(EnrollStudentCommand command)
    {
        try
        {
            // Verificar que el estudiante existe
            var student = await _studentRepository.GetByIdAsync(command.StudentId);
            if (student == null)
            {
                return new EnrollmentResponse
                {
                    Status = "Error",
                    Message = $"Estudiante con ID {command.StudentId} no encontrado"
                };
            }

            // Verificar que la carrera existe
            var career = await _careerRepository.GetByIdAsync(command.CareerId);
            if (career == null)
            {
                return new EnrollmentResponse
                {
                    Status = "Error",
                    Message = $"Carrera con ID {command.CareerId} no encontrada"
                };
            }

            // Verificar si ya existe matrícula activa
            var existingEnrollment = await _studentCareerRepository.ExistsActiveEnrollmentAsync(command.StudentId, command.CareerId);
            if (existingEnrollment)
            {
                return new EnrollmentResponse
                {
                    Status = "Error",
                    Message = "El estudiante ya está matriculado en esta carrera"
                };
            }

            // Verificar si el estudiante puede matricularse (reglas de negocio)
            if (!student.CanEnrollInCareer(career))
            {
                return new EnrollmentResponse
                {
                    Status = "Error",
                    Message = "El estudiante no cumple los requisitos para matricularse en esta carrera"
                };
            }

            // Crear la matrícula usando el dominio service
            var enrollment = await _studentDomainService.EnrollStudentInCareerAsync(student, career);

            // Persistir la matrícula
            var savedEnrollment = await _studentCareerRepository.AddEnrollmentAsync(enrollment);

            // Enviar notificación asíncrona a través de RabbitMQ
            var notificationMessage = new EnrollmentNotificationMessage
            {
                StudentEmail = savedEnrollment.Student?.Email?.Value ?? "",
                StudentName = savedEnrollment.Student?.FullName.FullDisplayName ?? "",
                StudentDni = savedEnrollment.Student?.Dni.Value ?? "",
                CareerName = savedEnrollment.Career?.Name ?? "",
                FacultyName = "", // La carrera no tiene navegación directa a Faculty
                EnrollmentDate = savedEnrollment.EnrollmentDate,
                NotificationType = "Enrollment"
            };

            await _messagePublisher.PublishEnrollmentNotificationAsync(notificationMessage);

            return new EnrollmentResponse
            {
                StudentId = savedEnrollment.StudentId,
                StudentName = savedEnrollment.Student?.FullName.FullDisplayName ?? "",
                StudentDni = savedEnrollment.Student?.Dni.Value ?? "",
                CareerId = savedEnrollment.CareerId,
                CareerName = savedEnrollment.Career?.Name ?? "",
                FacultyName = "", // La carrera no tiene navegación directa a Faculty en el dominio
                EnrollmentDate = savedEnrollment.EnrollmentDate,
                IsActive = savedEnrollment.IsActive,
                Status = "Success",
                Message = "Estudiante matriculado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentResponse
            {
                Status = "Error",
                Message = $"Error al matricular estudiante: {ex.Message}"
            };
        }
    }

    public async Task<EnrollmentResponse> UnenrollStudentFromCareerAsync(UnenrollStudentCommand command)
    {
        try
        {
            // Verificar que existe la matrícula
            var enrollment = await _studentCareerRepository.GetEnrollmentAsync(command.StudentId, command.CareerId);
            if (enrollment == null)
            {
                return new EnrollmentResponse
                {
                    Status = "Error",
                    Message = "No se encontró la matrícula especificada"
                };
            }

            if (!enrollment.IsActive)
            {
                return new EnrollmentResponse
                {
                    Status = "Error",
                    Message = "La matrícula ya está inactiva"
                };
            }

            // Desmatricular usando método del dominio
            enrollment.Unenroll();

            // Actualizar en la base de datos
            var updatedEnrollment = await _studentCareerRepository.UpdateEnrollmentAsync(enrollment);

            // Publicar mensaje para notificación asíncrona
            var unenrollmentMessage = new EnrollmentNotificationMessage
            {
                StudentEmail = updatedEnrollment.Student?.Email.Value ?? "",
                StudentName = updatedEnrollment.Student?.FullName.FullDisplayName ?? "",
                StudentDni = updatedEnrollment.Student?.Dni.Value ?? "",
                CareerName = updatedEnrollment.Career?.Name ?? "",
                FacultyName = "", // La carrera no tiene navegación directa a Faculty
                EnrollmentDate = updatedEnrollment.EnrollmentDate,
                NotificationType = "Unenrollment"
            };

            await _messagePublisher.PublishUnenrollmentNotificationAsync(unenrollmentMessage);

            return new EnrollmentResponse
            {
                StudentId = updatedEnrollment.StudentId,
                StudentName = updatedEnrollment.Student?.FullName.FullDisplayName ?? "",
                StudentDni = updatedEnrollment.Student?.Dni.Value ?? "",
                CareerId = updatedEnrollment.CareerId,
                CareerName = updatedEnrollment.Career?.Name ?? "",
                FacultyName = "", // La carrera no tiene navegación directa a Faculty en el dominio
                EnrollmentDate = updatedEnrollment.EnrollmentDate,
                IsActive = updatedEnrollment.IsActive,
                Status = "Success",
                Message = "Estudiante desmatriculado exitosamente"
            };
        }
        catch (Exception ex)
        {
            return new EnrollmentResponse
            {
                Status = "Error",
                Message = $"Error al desmatricular estudiante: {ex.Message}"
            };
        }
    }

    public async Task<IEnumerable<EnrollmentResponse>> GetStudentEnrollmentsAsync(int studentId)
    {
        try
        {
            var enrollments = await _studentCareerRepository.GetStudentEnrollmentsAsync(studentId);

            return enrollments.Select(e => new EnrollmentResponse
            {
                StudentId = e.StudentId,
                StudentName = e.Student?.FullName.FullDisplayName ?? "",
                StudentDni = e.Student?.Dni.Value ?? "",
                CareerId = e.CareerId,
                CareerName = e.Career?.Name ?? "",
                FacultyName = "", // La carrera no tiene navegación directa a Faculty en el dominio
                EnrollmentDate = e.EnrollmentDate,
                IsActive = e.IsActive,
                Status = "Success",
                Message = ""
            });
        }
        catch (Exception ex)
        {
            return new List<EnrollmentResponse>
            {
                new EnrollmentResponse
                {
                    Status = "Error",
                    Message = $"Error al obtener matrículas del estudiante: {ex.Message}"
                }
            };
        }
    }

    public async Task<IEnumerable<EnrollmentResponse>> GetCareerEnrollmentsAsync(int careerId)
    {
        try
        {
            var enrollments = await _studentCareerRepository.GetCareerEnrollmentsAsync(careerId);

            return enrollments.Select(e => new EnrollmentResponse
            {
                StudentId = e.StudentId,
                StudentName = e.Student?.FullName.FullDisplayName ?? "",
                StudentDni = e.Student?.Dni.Value ?? "",
                CareerId = e.CareerId,
                CareerName = e.Career?.Name ?? "",
                FacultyName = "", // La carrera no tiene navegación directa a Faculty en el dominio
                EnrollmentDate = e.EnrollmentDate,
                IsActive = e.IsActive,
                Status = "Success",
                Message = ""
            });
        }
        catch (Exception ex)
        {
            return new List<EnrollmentResponse>
            {
                new EnrollmentResponse
                {
                    Status = "Error",
                    Message = $"Error al obtener matrículas de la carrera: {ex.Message}"
                }
            };
        }
    }
}