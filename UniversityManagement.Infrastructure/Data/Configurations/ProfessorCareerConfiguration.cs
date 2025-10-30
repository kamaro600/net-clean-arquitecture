using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

public class ProfessorCareerConfiguration : IEntityTypeConfiguration<ProfessorCareer>
{
    public void Configure(EntityTypeBuilder<ProfessorCareer> builder)
    {
        builder.ToTable("profesor_carrera");

        // Clave compuesta
        builder.HasKey(pc => new { pc.ProfesorId, pc.CarreraId });

        builder.Property(pc => pc.ProfesorId)
            .HasColumnName("profesor_id");

        builder.Property(pc => pc.CarreraId)
            .HasColumnName("carrera_id");

        builder.Property(pc => pc.FechaAsignacion)
            .HasColumnName("fecha_asignacion")
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(pc => pc.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Relaciones
        builder.HasOne(pc => pc.Professor)
            .WithMany(p => p.ProfessorCareers)
            .HasForeignKey(pc => pc.ProfesorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(pc => pc.Career)
            .WithMany(c => c.ProfessorCareers)
            .HasForeignKey(pc => pc.CarreraId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}