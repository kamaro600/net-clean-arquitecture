using Microsoft.AspNetCore.Mvc;
using UniversityManagement.Application.Ports.In;
using UniversityManagement.Application.DTOs.Commands;
using UniversityManagement.Application.DTOs.Queries;
using UniversityManagement.Application.DTOs.Responses;

namespace UniversityManagement.WebApi.Controllers;

/// <summary>
/// Controlador para la gestión de carreras
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CareersController : ControllerBase
{
    private readonly ICareerUseCase _careerUseCase;

    public CareersController(ICareerUseCase careerUseCase)
    {
        _careerUseCase = careerUseCase;
    }

    /// <summary>
    /// Obtiene una carrera específica por su ID
    /// </summary>
    /// <param name="id">ID único de la carrera</param>
    /// <returns>Información detallada de la carrera incluyendo facultad y estadísticas</returns>
    /// <response code="200">Carrera encontrada exitosamente</response>
    /// <response code="404">No se encontró la carrera con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CareerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CareerResponse>> GetCareerById(int id)
    {
        var query = new GetCareerByIdQuery { Id = id };
        var result = await _careerUseCase.GetCareerByIdAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene una lista de carreras con filtro opcional por nombre
    /// </summary>
    /// <param name="searchTerm">Término opcional para filtrar carreras por nombre</param>
    /// <returns>Lista de carreras</returns>
    /// <response code="200">Lista de carreras obtenida exitosamente</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CareerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<CareerResponse>>> GetCareers([FromQuery] string? searchTerm = null)
    {
        var query = new GetCareersQuery { SearchTerm = searchTerm };
        var result = await _careerUseCase.GetCareersByNameAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Obtiene las carreras de una facultad específica
    /// </summary>
    /// <param name="facultyId">ID de la facultad</param>
    /// <returns>Lista de carreras de la facultad especificada</returns>
    /// <response code="200">Lista de carreras obtenida exitosamente</response>
    /// <response code="404">No se encontró la facultad especificada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpGet("faculty/{facultyId:int}")]
    [ProducesResponseType(typeof(List<CareerResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<CareerResponse>>> GetCareersByFaculty(int facultyId)
    {
        var query = new GetCareersByFacultyQuery { FacultyId = facultyId };
        var result = await _careerUseCase.GetCareersByFacultyAsync(query);
        return Ok(result);
    }

    /// <summary>
    /// Crea una nueva carrera en el sistema
    /// </summary>
    /// <param name="command">Datos de la carrera a crear</param>
    /// <returns>Información de la carrera creada</returns>
    /// <response code="201">Carrera creada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos o carrera duplicada</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPost]
    [ProducesResponseType(typeof(CareerResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CareerResponse>> CreateCareer([FromBody] CreateCareerCommand command)
    {
        var result = await _careerUseCase.CreateCareerAsync(command);
        return CreatedAtAction(
            nameof(GetCareerById), 
            new { id = result.Id }, 
            result);
    }

    /// <summary>
    /// Actualiza una carrera existente
    /// </summary>
    /// <param name="id">ID de la carrera a actualizar</param>
    /// <param name="command">Datos actualizados de la carrera</param>
    /// <returns>Información de la carrera actualizada</returns>
    /// <response code="200">Carrera actualizada exitosamente</response>
    /// <response code="400">Datos de entrada inválidos</response>
    /// <response code="404">No se encontró la carrera con el ID especificado</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(CareerResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<CareerResponse>> UpdateCareer(int id, [FromBody] UpdateCareerCommand command)
    {
        command.Id = id; // Asegurar que el ID del comando coincida con el de la URL
        var result = await _careerUseCase.UpdateCareerAsync(command);
        return Ok(result);
    }

    /// <summary>
    /// Elimina una carrera del sistema (eliminación lógica)
    /// </summary>
    /// <param name="id">ID de la carrera a eliminar</param>
    /// <returns>Confirmación de la eliminación</returns>
    /// <response code="200">Carrera eliminada exitosamente</response>
    /// <response code="404">No se encontró la carrera con el ID especificado</response>
    /// <response code="409">No se puede eliminar la carrera porque tiene estudiantes o profesores asignados</response>
    /// <response code="500">Error interno del servidor</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(DeletionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<DeletionResponse>> DeleteCareer(int id)
    {
        var command = new DeleteCareerCommand { Id = id };
        var result = await _careerUseCase.DeleteCareerAsync(command);
        return Ok(result);
    }
}