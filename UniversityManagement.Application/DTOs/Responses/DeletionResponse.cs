namespace UniversityManagement.Application.DTOs.Responses;

/// <summary>
/// Response para operaciones de eliminación
/// </summary>
public class DeletionResponse
{
    public bool IsSuccess { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool Data { get; set; }
    public List<string> Errors { get; set; } = new();

    public static DeletionResponse Success(string message = "Elemento eliminado exitosamente")
    {
        return new DeletionResponse
        {
            IsSuccess = true,
            Message = message,
            Data = true,
            Errors = new List<string>()
        };
    }

    public static DeletionResponse Failure(string message, List<string>? errors = null)
    {
        return new DeletionResponse
        {
            IsSuccess = false,
            Message = message,
            Data = false,
            Errors = errors ?? new List<string>()
        };
    }

    public static DeletionResponse NotFound(string message = "Elemento no encontrado")
    {
        return new DeletionResponse
        {
            IsSuccess = false,
            Message = message,
            Data = false,
            Errors = new List<string>()
        };
    }
}