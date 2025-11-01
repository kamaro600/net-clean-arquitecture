using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para ProfessorDataModel
/// </summary>
public class ProfessorDataConfiguration : IEntityTypeConfiguration<ProfessorDataModel>
{
    public void Configure(EntityTypeBuilder<ProfessorDataModel> builder)
    {
        builder.ToTable("profesor");

        builder.HasKey(e => e.ProfessorId);
        builder.Property(e => e.ProfessorId)
            .HasColumnName("profesor_id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FirstName)
            .HasColumnName("nombre")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.LastName)
            .HasColumnName("apellido")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Dni)
            .HasColumnName("dni")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(e => e.Email)
            .HasColumnName("email")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Phone)
            .HasColumnName("telefono")
            .HasMaxLength(20);

        builder.Property(e => e.Specialty)
            .HasColumnName("especialidad")
            .HasMaxLength(100);

        builder.Property(e => e.AcademicDegree)
            .HasColumnName("titulo_academico")
            .HasMaxLength(100);

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Índices únicos
        builder.HasIndex(e => e.Dni).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();

        // Configuración de relaciones
        builder.HasMany(e => e.ProfessorCareers)
            .WithOne(pc => pc.Professor)
            .HasForeignKey(pc => pc.ProfessorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}