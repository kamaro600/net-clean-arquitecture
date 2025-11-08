using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Models.ValueObjects;
using UniversityManagement.Infrastructure.Data.Configurations;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Data;

public class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
    {
    }

    // DbSets para modelos de datos (EF Core) - nueva arquitectura Domain/Data separada
    public DbSet<StudentDataModel> StudentsData { get; set; }
    public DbSet<ProfessorDataModel> ProfessorsData { get; set; }
    public DbSet<FacultyDataModel> FacultiesData { get; set; }
    public DbSet<CareerDataModel> CareersData { get; set; }
    public DbSet<StudentCareerDataModel> StudentCareersData { get; set; }
    public DbSet<ProfessorCareerDataModel> ProfessorCareersData { get; set; }
    
    // DbSet para auditoría
    public DbSet<AuditLog> AuditLogs { get; set; }
    

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
     
        // Configurar DataModels para persistencia
        ConfigureDataModels(modelBuilder);       
    }

    private void ConfigureDataModels(ModelBuilder modelBuilder)
    {
        // Aplicar configuraciones para los DataModels
        modelBuilder.ApplyConfiguration(new StudentDataConfiguration());
        modelBuilder.ApplyConfiguration(new ProfessorDataConfiguration());
        modelBuilder.ApplyConfiguration(new FacultyDataConfiguration());
        modelBuilder.ApplyConfiguration(new CareerDataConfiguration());
        modelBuilder.ApplyConfiguration(new StudentCareerDataConfiguration());
        modelBuilder.ApplyConfiguration(new ProfessorCareerDataConfiguration());
        
        // Configuración para auditoría
        modelBuilder.ApplyConfiguration(new AuditLogConfiguration());
    }

}