using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

public class CareerConfiguration : IEntityTypeConfiguration<Career>
{
    public void Configure(EntityTypeBuilder<Career> builder)
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
            .HasColumnType("text");

        builder.Property(e => e.SemesterDuration)
            .HasColumnName("duracion_semestres")
            .IsRequired();

        builder.Property(e => e.AwardedTitle)
            .HasColumnName("titulo_otorgado")
            .HasMaxLength(100);

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Relación con Faculty
        builder.HasOne(e => e.Faculty)
            .WithMany(f => f.Careers)
            .HasForeignKey(e => e.FacultyId)
            .HasConstraintName("fk_facultad")
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único para el nombre
        builder.HasIndex(e => e.Name)
            .IsUnique()
            .HasDatabaseName("uk_carrera_nombre");
    }
}