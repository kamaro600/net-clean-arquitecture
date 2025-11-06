namespace UniversityManagement.Application.DTOs.Messages;

/// <summary>
/// Mensaje para notificaciones de matrícula a través de RabbitMQ
/// </summary>
public class EnrollmentNotificationMessage
{
    public string StudentEmail { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentDni { get; set; } = string.Empty;
    public string CareerName { get; set; } = string.Empty;
    public string FacultyName { get; set; } = string.Empty;
    public DateTime EnrollmentDate { get; set; }
    public string NotificationType { get; set; } = string.Empty; // "Enrollment" o "Unenrollment"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string MessageId { get; set; } = Guid.NewGuid().ToString();
}