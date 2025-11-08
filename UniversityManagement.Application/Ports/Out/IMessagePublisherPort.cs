using UniversityManagement.Application.DTOs.Messages;

namespace UniversityManagement.Application.Ports.Out;

/// <summary>
/// Puerto para el servicio de mensajería (RabbitMQ)
/// </summary>
public interface IMessagePublisherPort
{
    Task PublishEnrollmentNotificationAsync(EnrollmentNotificationMessage message);
    Task PublishUnenrollmentNotificationAsync(EnrollmentNotificationMessage message);
}