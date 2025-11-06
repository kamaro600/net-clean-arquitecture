using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.Ports.In;

namespace UniversityManagement.WebApi.Controllers;

/// <summary>
/// Controlador para gestionar matrículas de estudiantes
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentUseCase _enrollmentUseCase;

    public EnrollmentController(IEnrollmentUseCase enrollmentUseCase)
    {
        _enrollmentUseCase = enrollmentUseCase;
    }

    /// <summary>
    /// Matricular un estudiante en una carrera
    /// </summary>
    /// <param name="command">Datos de la matrícula</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("enroll")]
    public async Task<ActionResult<EnrollmentResponse>> EnrollStudent([FromBody] EnrollStudentCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _enrollmentUseCase.EnrollStudentInCareerAsync(command);
        
        if (result.Status == "Error")
        {
            return BadRequest(result);
        }

        return CreatedAtAction(
            nameof(GetStudentEnrollments), 
            new { studentId = result.StudentId }, 
            result
        );
    }

    /// <summary>
    /// Desmatricular un estudiante de una carrera
    /// </summary>
    /// <param name="command">Datos de la desmatrícula</param>
    /// <returns>Resultado de la operación</returns>
    [HttpPost("unenroll")]
    public async Task<ActionResult<EnrollmentResponse>> UnenrollStudent([FromBody] UnenrollStudentCommand command)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var result = await _enrollmentUseCase.UnenrollStudentFromCareerAsync(command);
        
        if (result.Status == "Error")
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    /// <summary>
    /// Obtener todas las matrículas de un estudiante
    /// </summary>
    /// <param name="studentId">ID del estudiante</param>
    /// <returns>Lista de matrículas del estudiante</returns>
    [HttpGet("student/{studentId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentResponse>>> GetStudentEnrollments(int studentId)
    {
        if (studentId <= 0)
        {
            return BadRequest("El ID del estudiante debe ser mayor a 0");
        }

        var enrollments = await _enrollmentUseCase.GetStudentEnrollmentsAsync(studentId);
        return Ok(enrollments);
    }

    /// <summary>
    /// Obtener todas las matrículas de una carrera
    /// </summary>
    /// <param name="careerId">ID de la carrera</param>
    /// <returns>Lista de matrículas de la carrera</returns>
    [HttpGet("career/{careerId}")]
    public async Task<ActionResult<IEnumerable<EnrollmentResponse>>> GetCareerEnrollments(int careerId)
    {
        if (careerId <= 0)
        {
            return BadRequest("El ID de la carrera debe ser mayor a 0");
        }

        var enrollments = await _enrollmentUseCase.GetCareerEnrollmentsAsync(careerId);
        return Ok(enrollments);
    }

    /// <summary>
    /// Verificar si un estudiante está matriculado en una carrera específica
    /// </summary>
    /// <param name="studentId">ID del estudiante</param>
    /// <param name="careerId">ID de la carrera</param>
    /// <returns>Estado de la matrícula</returns>
    [HttpGet("check/{studentId}/{careerId}")]
    public async Task<ActionResult<bool>> CheckEnrollment(int studentId, int careerId)
    {
        if (studentId <= 0 || careerId <= 0)
        {
            return BadRequest("Los IDs deben ser mayores a 0");
        }

        var enrollments = await _enrollmentUseCase.GetStudentEnrollmentsAsync(studentId);
        var isEnrolled = enrollments.Any(e => e.CareerId == careerId && e.IsActive);
        
        return Ok(isEnrolled);
    }
}