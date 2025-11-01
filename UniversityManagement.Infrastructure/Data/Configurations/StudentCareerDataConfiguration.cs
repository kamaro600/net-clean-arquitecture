using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para StudentCareerDataModel (relación N:M)
/// </summary>
public class StudentCareerDataConfiguration : IEntityTypeConfiguration<StudentCareerDataModel>
{
    public void Configure(EntityTypeBuilder<StudentCareerDataModel> builder)
    {
        builder.ToTable("estudiante_carrera");

        // Clave primaria compuesta
        builder.HasKey(e => new { e.StudentId, e.CareerId });

        builder.Property(e => e.StudentId)
            .HasColumnName("estudiante_id")
            .IsRequired();

        builder.Property(e => e.CareerId)
            .HasColumnName("carrera_id")
            .IsRequired();

        builder.Property(e => e.EnrollmentDate)
            .HasColumnName("fecha_inscripcion")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Configuración de relaciones
        builder.HasOne(e => e.Student)
            .WithMany(s => s.StudentCareers)
            .HasForeignKey(e => e.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Career)
            .WithMany(c => c.StudentCareers)
            .HasForeignKey(e => e.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(e => e.StudentId);
        builder.HasIndex(e => e.CareerId);
        builder.HasIndex(e => e.EnrollmentDate);
    }
}