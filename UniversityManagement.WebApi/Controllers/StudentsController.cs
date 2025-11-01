using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.DTOs.Responses;
using UniversityManagement.Application.Ports.In;

namespace UniversityManagement.WebApi.Controllers;

/// <summary>
/// Controlador para gestión de estudiantes
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StudentsController : ControllerBase
{
    private readonly IStudentUseCase _studentUseCase;

    public StudentsController(IStudentUseCase studentUseCase)
    {
        _studentUseCase = studentUseCase;
    }

    /// <summary>
    /// Obtiene un estudiante por su ID
    /// </summary>
    /// <param name="id">ID del estudiante</param>
    /// <returns>Información del estudiante</returns>
    /// <response code="200">Estudiante encontrado exitosamente</response>
    /// <response code="404">Estudiante no encontrado</response>
    /// <response code="400">ID inválido</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentResponse>> GetStudentById(int id)
    {
        var query = new GetStudentByIdQuery(id);
        var response = await _studentUseCase.GetStudentByIdAsync(query);
        
        return Ok(response);
    }

    /// <summary>
    /// Crea un nuevo estudiante
    /// </summary>
    /// <param name="command">Datos del estudiante a crear</param>
    /// <returns>Estudiante creado</returns>
    /// <response code="201">Estudiante creado exitosamente</response>
    /// <response code="400">Datos inválidos o estudiante duplicado</response>
    [HttpPost]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentResponse>> CreateStudent([FromBody] CreateStudentCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _studentUseCase.CreateStudentAsync(command);
        
        return CreatedAtAction(nameof(GetStudentById), new { id = response.Id }, response);
    }

    /// <summary>
    /// Actualiza un estudiante existente
    /// </summary>
    /// <param name="id">ID del estudiante a actualizar</param>
    /// <param name="command">Nuevos datos del estudiante</param>
    /// <returns>Estudiante actualizado</returns>
    /// <response code="200">Estudiante actualizado exitosamente</response>
    /// <response code="404">Estudiante no encontrado</response>
    /// <response code="400">Datos inválidos</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(StudentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<StudentResponse>> UpdateStudent(int id, [FromBody] UpdateStudentCommand command)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        command.Id = id;
        var response = await _studentUseCase.UpdateStudentAsync(command);
        
        return Ok(response);
    }

    /// <summary>
    /// Elimina un estudiante
    /// </summary>
    /// <param name="id">ID del estudiante a eliminar</param>
    /// <returns>Confirmación de eliminación</returns>
    /// <response code="200">Estudiante eliminado exitosamente</response>
    /// <response code="404">Estudiante no encontrado</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(DeletionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DeletionResponse>> DeleteStudent(int id)
    {
        var command = new DeleteStudentCommand { Id = id };
        var response = await _studentUseCase.DeleteStudentAsync(command);
        
        return Ok(response);
    }
}