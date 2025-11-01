using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para ProfessorCareerDataModel (relación N:M)
/// </summary>
public class ProfessorCareerDataConfiguration : IEntityTypeConfiguration<ProfessorCareerDataModel>
{
    public void Configure(EntityTypeBuilder<ProfessorCareerDataModel> builder)
    {
        builder.ToTable("profesor_carrera");

        // Clave primaria compuesta
        builder.HasKey(e => new { e.ProfessorId, e.CareerId });

        builder.Property(e => e.ProfessorId)
            .HasColumnName("profesor_id")
            .IsRequired();

        builder.Property(e => e.CareerId)
            .HasColumnName("carrera_id")
            .IsRequired();

        builder.Property(e => e.AssignmentDate)
            .HasColumnName("fecha_asignacion")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP")
            .IsRequired();

        builder.Property(e => e.IsActive)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Configuración de relaciones
        builder.HasOne(e => e.Professor)
            .WithMany(p => p.ProfessorCareers)
            .HasForeignKey(e => e.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Career)
            .WithMany(c => c.ProfessorCareers)
            .HasForeignKey(e => e.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        // Índices
        builder.HasIndex(e => e.ProfessorId);
        builder.HasIndex(e => e.CareerId);
        builder.HasIndex(e => e.AssignmentDate);
    }
}