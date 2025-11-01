using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.DTOs.Responses;

namespace UniversityManagement.WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de facultades
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FacultiesController : ControllerBase
{
    private readonly IFacultyUseCase _facultyUseCase;

    public FacultiesController(IFacultyUseCase facultyUseCase)
    {
        _facultyUseCase = facultyUseCase;
    }

    /// <summary>
    /// Obtiene una facultad específica por su ID
    /// </summary>
    /// <param name="id">ID único de la facultad</param>
    /// <returns>Información detallada de la facultad incluyendo carreras y estadísticas</returns>
    /// <response code="200">Facultad encontrada exitosamente</response>
    /// <response code="404">No se encontró la facultad con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FacultyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FacultyResponse>> GetFacultyById(int id)
    {
        var query = new GetFacultyByIdQuery { Id = id };
        var result = await _facultyUseCase.GetFacultyByIdAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una lista de facultades con filtro opcional por nombre
    /// </summary>
    /// <param name="searchTerm">Término opcional para filtrar facultades por nombre</param>
    /// <returns>Lista de facultades</returns>
    /// <response code="200">Lista de facultades obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<FacultyResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<FacultyResponse>>> GetFaculties([FromQuery] string? searchTerm = null)
    {
        var query = new GetFacultiesQuery { SearchTerm = searchTerm };
        var result = await _facultyUseCase.GetFacultiesByNameAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva facultad en el sistema
    /// </summary>
    /// <param name="command">Datos de la facultad a crear</param>
    /// <returns>Información de la facultad creada</returns>
    /// <response code="201">Facultad creada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o facultad duplicada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(FacultyResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FacultyResponse>> CreateFaculty([FromBody] CreateFacultyCommand command)
    {
        var result = await _facultyUseCase.CreateFacultyAsync(command);
        return CreatedAtAction(
            nameof(GetFacultyById), 
            new { id = result.Id }, 
            result);
    }

    /// <summary>
    /// Actualiza una facultad existente
    /// </summary>
    /// <param name="id">ID de la facultad a actualizar</param>
    /// <param name="command">Datos actualizados de la facultad</param>
    /// <returns>Información de la facultad actualizada</returns>
    /// <response code="200">Facultad actualizada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="404">No se encontró la facultad con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(FacultyResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FacultyResponse>> UpdateFaculty(int id, [FromBody] UpdateFacultyCommand command)
    {
        command.Id = id; // Asegurar que el ID del comando coincida con el de la URL
        var result = await _facultyUseCase.UpdateFacultyAsync(command);
        return Ok(result);
    }

    /// <summary>
    /// Elimina una facultad del sistema (eliminación lógica)
    /// </summary>
    /// <param name="id">ID de la facultad a eliminar</param>
    /// <returns>Confirmación de la eliminación</returns>
    /// <response code="200">Facultad eliminada exitosamente</response>
    /// <response code="404">No se encontró la facultad con el ID especificado</response>
    /// <response code="409">No se puede eliminar la facultad porque tiene carreras asociadas</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeletionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeletionResponse>> DeleteFaculty(int id)
    {
        var command = new DeleteFacultyCommand { Id = id };
        var result = await _facultyUseCase.DeleteFacultyAsync(command);
        return Ok(result);
    }
}