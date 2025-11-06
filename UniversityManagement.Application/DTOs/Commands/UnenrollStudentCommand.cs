using System.ComponentModel.DataAnnotations;

namespace UniversityManagement.Application.DTOs.Commands;

/// <summary>
/// Comando para desmatricular un estudiante de una carrera
/// </summary>
public class UnenrollStudentCommand
{
    /// <summary>
    /// ID del estudiante a desmatricular
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID del estudiante debe ser mayor a 0")]
    public int StudentId { get; set; }

    /// <summary>
    /// ID de la carrera de la que se desmatriculará
    /// </summary>
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "El ID de la carrera debe ser mayor a 0")]
    public int CareerId { get; set; }

    /// <summary>
    /// Motivo de la desmatrícula (opcional)
    /// </summary>
    public string? Reason { get; set; }
}