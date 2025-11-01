using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Infrastructure.Data.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

/// <summary>
/// Configuración de EF Core para FacultyDataModel
/// </summary>
public class FacultyDataConfiguration : IEntityTypeConfiguration<FacultyDataModel>
{
    public void Configure(EntityTypeBuilder<FacultyDataModel> builder)
    {
        builder.ToTable("facultad");

        builder.HasKey(e => e.FacultyId);
        builder.Property(e => e.FacultyId)
            .HasColumnName("facultad_id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Name)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Description)
            .HasColumnName("descripcion")
            .HasMaxLength(1000);

        builder.Property(e => e.Location)
            .HasColumnName("ubicacion")
            .HasMaxLength(200);

        builder.Property(e => e.Dean)
            .HasColumnName("decano")
            .HasMaxLength(100);

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Índices únicos
        builder.HasIndex(e => e.Name).IsUnique();

        // Configuración de relaciones
        builder.HasMany(e => e.Careers)
            .WithOne(c => c.Faculty)
            .HasForeignKey(c => c.FacultyId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}