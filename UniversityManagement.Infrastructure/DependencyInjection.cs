using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using UniversityManagement.Domain.Repositories;
using UniversityManagement.Domain.Services.Interfaces;
using UniversityManagement.Infrastructure.Data;
using UniversityManagement.Infrastructure.Persistence.Repositories;
using UniversityManagement.Infrastructure.Adapters.Out;
using UniversityManagement.Domain.Services;
using UniversityManagement.Application.Ports.Out;

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

        // Registrar repositorios (Implementaciones de Domain Interfaces)
        services.AddScoped<IStudentRepository, StudentRepository>();
        services.AddScoped<ICareerRepository, CareerRepository>();
        services.AddScoped<IFacultyRepository, FacultyRepository>();
        services.AddScoped<IProfessorRepository, ProfessorRepository>();

        // Registrar servicios de dominio
        services.AddScoped<IStudentDomainService, StudentDomainService>();
        services.AddScoped<IProfessorDomainService, ProfessorDomainService>();
        
        // Registrar adapters para Application Ports
        services.AddScoped<IEmailNotificationPort, EmailNotificationAdapter>();
        services.AddScoped<ISmsNotificationPort, SmsNotificationAdapter>();

        // Registrar dependencias de adapters
        services.AddTransient<System.Net.Mail.SmtpClient>();

        return services;
    }
}