using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.DTOs.Responses;

namespace UniversityManagement.WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de profesores
/// Expone endpoints RESTful para operaciones CRUD de profesores
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class ProfessorsController : ControllerBase
{
    private readonly IProfessorUseCase _professorUseCase;

    public ProfessorsController(IProfessorUseCase professorUseCase)
    {
        _professorUseCase = professorUseCase;
    }

    /// <summary>
    /// Obtiene un profesor específico por su ID
    /// </summary>
    /// <param name="id">ID único del profesor</param>
    /// <returns>Información detallada del profesor incluyendo carreras asignadas</returns>
    /// <response code="200">Profesor encontrado exitosamente</response>
    /// <response code="404">No se encontró el profesor con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProfessorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProfessorResponse>> GetProfessorById(int id)
    {
        var query = new GetProfessorByIdQuery { Id = id };
        var result = await _professorUseCase.GetProfessorByIdAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una lista de todos los profesores
    /// </summary>
    /// <param name="searchTerm">Término opcional para filtrar profesores por nombre, apellido o especialidad</param>
    /// <returns>Lista paginada de profesores</returns>
    /// <response code="200">Lista de profesores obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<ProfessorResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<ProfessorResponse>>> GetProfessors([FromQuery] string? searchTerm = null)
    {
        var query = new GetProfessorsQuery { SearchTerm = searchTerm };
        var result = await _professorUseCase.GetProfessorsAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Crea un nuevo profesor en el sistema
    /// </summary>
    /// <param name="command">Datos del profesor a crear</param>
    /// <returns>Información del profesor creado</returns>
    /// <response code="201">Profesor creado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o profesor duplicado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(ProfessorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProfessorResponse>> CreateProfessor([FromBody] CreateProfessorCommand command)
    {
        var result = await _professorUseCase.CreateProfessorAsync(command);
        return CreatedAtAction(
            nameof(GetProfessorById), 
            new { id = result.Id }, 
            result);
    }

    /// <summary>
    /// Actualiza un profesor existente
    /// </summary>
    /// <param name="id">ID del profesor a actualizar</param>
    /// <param name="command">Datos actualizados del profesor</param>
    /// <returns>Información del profesor actualizado</returns>
    /// <response code="200">Profesor actualizado exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="404">No se encontró el profesor con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProfessorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProfessorResponse>> UpdateProfessor(int id, [FromBody] UpdateProfessorCommand command)
    {
        command.Id = id; // Asegurar que el ID del comando coincida con el de la URL
        var result = await _professorUseCase.UpdateProfessorAsync(command);
        return Ok(result);
    }

    /// <summary>
    /// Elimina un profesor del sistema (eliminación lógica)
    /// </summary>
    /// <param name="id">ID del profesor a eliminar</param>
    /// <returns>Confirmación de la eliminación</returns>
    /// <response code="200">Profesor eliminado exitosamente</response>
    /// <response code="404">No se encontró el profesor con el ID especificado</response>
    /// <response code="409">No se puede eliminar el profesor porque tiene carreras asignadas</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeletionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeletionResponse>> DeleteProfessor(int id)
    {
        var command = new DeleteProfessorCommand { Id = id };
        var result = await _professorUseCase.DeleteProfessorAsync(command);
        return Ok(result);
    }
}