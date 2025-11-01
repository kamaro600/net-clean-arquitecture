using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

public class FacultyConfiguration : IEntityTypeConfiguration<Faculty>
{
    public void Configure(EntityTypeBuilder<Faculty> builder)
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
            .HasColumnType("text");

        builder.Property(e => e.Location)
            .HasColumnName("ubicacion")
            .HasMaxLength(100);

        builder.Property(e => e.Dean)
            .HasColumnName("decano")
            .HasMaxLength(100);

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Índice único para el nombre
        builder.HasIndex(e => e.Name).IsUnique();
    }
}