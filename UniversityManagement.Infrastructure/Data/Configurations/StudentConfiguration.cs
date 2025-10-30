using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.ToTable("estudiante");

        builder.HasKey(e => e.EstudianteId);
        builder.Property(e => e.EstudianteId)
            .HasColumnName("estudiante_id")
            .ValueGeneratedOnAdd();

        builder.Property(e => e.Nombre)
            .HasColumnName("nombre")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(e => e.Apellido)
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

        builder.Property(e => e.Telefono)
            .HasColumnName("telefono")
            .HasMaxLength(20);

        builder.Property(e => e.FechaNacimiento)
            .HasColumnName("fecha_nacimiento")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(e => e.Direccion)
            .HasColumnName("direccion")
            .HasMaxLength(200);

        builder.Property(e => e.FechaRegistro)
            .HasColumnName("fecha_registro")
            .HasColumnType("timestamp without time zone")
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.Property(e => e.Activo)
            .HasColumnName("activo")
            .HasDefaultValue(true);

        // Ignorar Value Objects - EF Core no debe mapearlos
        builder.Ignore(e => e.FullNameVO);
        builder.Ignore(e => e.DniVO);
        builder.Ignore(e => e.EmailVO);
        builder.Ignore(e => e.PhoneVO);
        builder.Ignore(e => e.AddressVO);

        // Índices únicos
        builder.HasIndex(e => e.Dni).IsUnique();
        builder.HasIndex(e => e.Email).IsUnique();
    }
}