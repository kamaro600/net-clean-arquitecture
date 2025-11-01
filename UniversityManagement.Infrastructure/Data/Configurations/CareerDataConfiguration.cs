using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para CareerDataModel
/// </summary>
public class CareerDataConfiguration : IEntityTypeConfiguration<CareerDataModel>
{
    public void Configure(EntityTypeBuilder<CareerDataModel> builder)
    {
        builder.ToTable("carrera");

        builder.HasKey(e => e.CareerId);
        builder.Property(e => e.CareerId)
            .HasColumnName("carrera_id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FacultyId)
            .HasColumnName("facultad_id")
            .IsRequired();

        builder.Property(e => e.Name)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("descripcion")
            .HasMaxLength(500);

        builder.Property(e => e.SemesterDuration)
            .HasColumnName("duracion_semestres")
            .IsRequired();

        builder.Property(e => e.AwardedTitle)
            .HasColumnName("titulo_otorgado")
            .HasMaxLength(150);

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Índices únicos
        builder.HasIndex(e => new { e.Name, e.FacultyId }).IsUnique();

        // Configuración de relaciones
        builder.HasOne(e => e.Faculty)
            .WithMany(f => f.Careers)
            .HasForeignKey(e => e.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.StudentCareers)
            .WithOne(sc => sc.Career)
            .HasForeignKey(sc => sc.CareerId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.ProfessorCareers)
            .WithOne(pc => pc.Career)
            .HasForeignKey(pc => pc.CareerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}