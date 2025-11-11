using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Services.Interfaces;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Repositories;
using UniversityManagement.Infrastructure.Adapters.Out;
using UniversityManagement.Infrastructure.Mappers;
using UniversityManagement.Domain.Services;
using UniversityManagement.Application.Ports.Out;
using UniversityManagement.Infrastructure.Configuration;
using UniversityManagement.Infrastructure.Services;

namespace UniversityManagement.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra todos los servicios de Infrastructure
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar Entity Framework
        services.AddDbContext<UniversityDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        // Configurar RabbitMQ
        services.Configure<RabbitMQSettings>(configuration.GetSection("RabbitMQ"));
        
        // Configurar Kafka
        services.Configure<KafkaSettings>(configuration.GetSection("Kafka"));
        
        // Configurar SMTP
        services.Configure<SmtpSettings>(configuration.GetSection("SmtpSettings"));
        
        // Registrar servicios de RabbitMQ
        services.AddSingleton<RabbitMQConnectionService>();
        services.AddScoped<IMessagePublisherPort, RabbitMQMessagePublisherAdapter>();
        services.AddHostedService<RabbitMQConsumerService>();
        
        // Registrar servicios de Kafka para auditoría
        services.AddScoped<IAuditPublisherPort, KafkaAuditPublisherAdapter>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddHostedService<KafkaAuditConsumerService>();
        services.AddHostedService<KafkaTopicSetupService>(); // Configuración automática de topics

        // Registrar repositorios (Implementaciones de Domain Interfaces)
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICareerRepository, CareerRepository>();
        services.AddScoped<IFacultyRepository, FacultyRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();
        services.AddScoped<IStudentCareerRepository, StudentCareerRepository>();

        // Registrar mappers
        services.AddScoped<StudentCareerMapper>();

        // Registrar servicios de dominio
        services.AddScoped<IStudentDomainService, StudentDomainService>();
        services.AddScoped<IProfessorDomainService, ProfessorDomainService>();
        
        // Registrar adapters para Application Ports
        services.AddScoped<IEmailNotificationPort, EmailNotificationAdapter>();
        services.AddScoped<ISmsNotificationPort, SmsNotificationAdapter>();

        return services;
    }
}