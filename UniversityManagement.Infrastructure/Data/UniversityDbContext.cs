using Microsoft.EntityFrameworkCore;
using UniversityManagement.Domain.Models;
using UniversityManagement.Domain.Models.ValueObjects;
using UniversityManagement.Infrastructure.Data.Configurations;

namespace UniversityManagement.Infrastructure.Data;

public class UniversityDbContext : DbContext
{
    public UniversityDbContext(DbContextOptions<UniversityDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Professor> Professors { get; set; }
    public DbSet<Faculty> Faculties { get; set; }
    public DbSet<Career> Careers { get; set; }
    public DbSet<StudentCareer> StudentCareers { get; set; }
    public DbSet<ProfessorCareer> ProfessorCareers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ignorar todos los Value Objects para que EF Core no los mapee
        modelBuilder.Ignore<FullName>();
        modelBuilder.Ignore<Dni>();
        modelBuilder.Ignore<Email>();
        modelBuilder.Ignore<Phone>();
        modelBuilder.Ignore<Address>();

        // Aplicar configuraciones
        modelBuilder.ApplyConfiguration(new StudentConfiguration());
        modelBuilder.ApplyConfiguration(new ProfessorConfiguration());
        modelBuilder.ApplyConfiguration(new FacultyConfiguration());
        modelBuilder.ApplyConfiguration(new CareerConfiguration());
        modelBuilder.ApplyConfiguration(new StudentCareerConfiguration());
        modelBuilder.ApplyConfiguration(new ProfessorCareerConfiguration());
    }
}