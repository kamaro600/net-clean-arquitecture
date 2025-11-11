using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using UniversityManagement.Domain.Exceptions;

namespace UniversityManagement.WebApi.Middleware;

/// <summary>
/// Middleware para manejo centralizado de excepciones siguiendo Clean Architecture
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = exception switch
        {
            // Excepciones de Student
            StudentNotFoundException ex => CreateErrorResponse(
                StatusCodes.Status404NotFound,
                "STUDENT_NOT_FOUND",
                ex.Message
            ),
            DuplicateStudentException ex => CreateErrorResponse(
                StatusCodes.Status409Conflict,
                "DUPLICATE_STUDENT",
                ex.Message
            ),
            InvalidEmailException ex => CreateErrorResponse(
                StatusCodes.Status400BadRequest,
                "INVALID_EMAIL",
                ex.Message
            ),
            InvalidDniException ex => CreateErrorResponse(
                StatusCodes.Status400BadRequest,
                "INVALID_DNI",
                ex.Message
            ),
            StudentTooYoungException ex => CreateErrorResponse(
                StatusCodes.Status400BadRequest,
                "STUDENT_TOO_YOUNG",
                ex.Message
            ),

            // Excepciones de Professor
            ProfessorNotFoundException ex => CreateErrorResponse(
                StatusCodes.Status404NotFound,
                "PROFESSOR_NOT_FOUND",
                ex.Message
            ),
            DuplicateProfessorException ex => CreateErrorResponse(
                StatusCodes.Status409Conflict,
                "DUPLICATE_PROFESSOR",
                ex.Message
            ),
            ProfessorAlreadyAssignedException ex => CreateErrorResponse(
                StatusCodes.Status409Conflict,
                "PROFESSOR_ALREADY_ASSIGNED",
                ex.Message
            ),

            // Excepciones de Career
            CareerNotFoundException ex => CreateErrorResponse(
                StatusCodes.Status404NotFound,
                "CAREER_NOT_FOUND",
                ex.Message
            ),
            DuplicateCareerException ex => CreateErrorResponse(
                StatusCodes.Status409Conflict,
                "DUPLICATE_CAREER",
                ex.Message
            ),

            // Excepciones de Faculty
            FacultyNotFoundException ex => CreateErrorResponse(
                StatusCodes.Status404NotFound,
                "FACULTY_NOT_FOUND",
                ex.Message
            ),
            DuplicateFacultyException ex => CreateErrorResponse(
                StatusCodes.Status409Conflict,
                "DUPLICATE_FACULTY",
                ex.Message
            ),

            // Excepciones de dominio genéricas
            DomainException ex => CreateErrorResponse(
                StatusCodes.Status400BadRequest,
                "DOMAIN_ERROR",
                ex.Message
            ),

            // Excepciones de validación de argumentos
            ArgumentNullException ex => CreateErrorResponse(
                StatusCodes.Status400BadRequest,
                "INVALID_INPUT",
                $"El parámetro '{ex.ParamName}' es requerido."
            ),
            ArgumentException ex => CreateErrorResponse(
                StatusCodes.Status400BadRequest,
                "INVALID_INPUT",
                ex.Message
            ),

            // Excepción genérica para errores no controlados
            _ => CreateErrorResponse(
                StatusCodes.Status500InternalServerError,
                "INTERNAL_SERVER_ERROR",
                "Ha ocurrido un error inesperado en el servidor."
            )
        };

        context.Response.StatusCode = response.StatusCode;

        // Log del error
        LogException(exception, response.StatusCode);

        var jsonResponse = JsonSerializer.Serialize(response.Body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(jsonResponse);
    }

    private static (int StatusCode, object Body) CreateErrorResponse(int statusCode, string errorCode, string message)
    {
        var body = new
        {
            Error = new
            {
                Code = errorCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            }
        };

        return (statusCode, body);
    }

    private void LogException(Exception exception, int statusCode)
    {
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Error interno del servidor: {Message}", exception.Message);
        }
        else if (statusCode >= 400)
        {
            _logger.LogWarning("Error de cliente ({StatusCode}): {Message}", statusCode, exception.Message);
        }
        else
        {
            _logger.LogInformation("Excepción manejada ({StatusCode}): {Message}", statusCode, exception.Message);
        }
    }
}
