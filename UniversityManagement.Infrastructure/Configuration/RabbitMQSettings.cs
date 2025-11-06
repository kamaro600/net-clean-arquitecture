namespace UniversityManagement.Infrastructure.Configuration;

/// <summary>
/// Configuración para RabbitMQ
/// </summary>
public class RabbitMQSettings
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string ExchangeName { get; set; } = "university.notifications";
    public string EnrollmentQueueName { get; set; } = "enrollment.notifications";
    public string UnenrollmentQueueName { get; set; } = "unenrollment.notifications";
    public string EnrollmentRoutingKey { get; set; } = "enrollment.created";
    public string UnenrollmentRoutingKey { get; set; } = "enrollment.deleted";
}