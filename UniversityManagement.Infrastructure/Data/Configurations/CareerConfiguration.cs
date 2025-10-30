using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

public class CareerConfiguration : IEntityTypeConfiguration<Career>
{
    public void Configure(EntityTypeBuilder<Career> builder)
    {
        builder.ToTable("carrera");

        builder.HasKey(e => e.CarreraId);
        builder.Property(e => e.CarreraId)
            .HasColumnName("carrera_id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.FacultadId)
            .HasColumnName("facultad_id")
            .IsRequired();

        builder.Property(e => e.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(e => e.Descripcion)
            .HasColumnName("descripcion")
            .HasColumnType("text");

        builder.Property(e => e.DuracionSemestres)
            .HasColumnName("duracion_semestres")
            .IsRequired();

        builder.Property(e => e.TituloOtorgado)
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
            .HasForeignKey(e => e.FacultadId)
            .HasConstraintName("fk_facultad")
            .OnDelete(DeleteBehavior.Restrict);

        // Índice único para el nombre
        builder.HasIndex(e => e.Nombre)
            .IsUnique()
            .HasDatabaseName("uk_carrera_nombre");
    }
}