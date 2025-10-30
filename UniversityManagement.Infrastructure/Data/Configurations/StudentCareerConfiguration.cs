using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

public class StudentCareerConfiguration : IEntityTypeConfiguration<StudentCareer>
{
    public void Configure(EntityTypeBuilder<StudentCareer> builder)
    {
        builder.ToTable("estudiante_carrera");

        // Clave compuesta
        builder.HasKey(sc => new { sc.EstudianteId, sc.CarreraId });

        builder.Property(sc => sc.EstudianteId)
            .HasColumnName("estudiante_id");

        builder.Property(sc => sc.CarreraId)
            .HasColumnName("carrera_id");

        builder.Property(sc => sc.FechaInscripcion)
            .HasColumnName("fecha_inscripcion")
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(sc => sc.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(sc => sc.Student)
            .WithMany(s => s.StudentCareers)
            .HasForeignKey(sc => sc.EstudianteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.Career)
            .WithMany(c => c.StudentCareers)
            .HasForeignKey(sc => sc.CarreraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}