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
        builder.HasKey(sc => new { sc.StudentId, sc.CareerId });

        builder.Property(sc => sc.StudentId)
            .HasColumnName("estudiante_id");

        builder.Property(sc => sc.CareerId)
            .HasColumnName("carrera_id");

        builder.Property(sc => sc.EnrollmentDate)
            .HasColumnName("fecha_inscripcion")
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(sc => sc.IsActive)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(sc => sc.Student)
            .WithMany(s => s.StudentCareers)
            .HasForeignKey(sc => sc.StudentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(sc => sc.Career)
            .WithMany(c => c.StudentCareers)
            .HasForeignKey(sc => sc.CareerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}