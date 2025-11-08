using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using UniversityManagement.Domain.Models;

namespace UniversityManagement.Infrastructure.Data.Configurations;

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> builder)
    {
        builder.ToTable("audit_logs");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Id)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(a => a.EventType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.EntityId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(a => a.Action)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(a => a.OldValues)
            .HasColumnType("text");

        builder.Property(a => a.NewValues)
            .HasColumnType("text");

        builder.Property(a => a.UserId)
            .HasMaxLength(36);

        builder.Property(a => a.UserName)
            .HasMaxLength(100);

        builder.Property(a => a.IpAddress)
            .HasMaxLength(45);

        builder.Property(a => a.UserAgent)
            .HasMaxLength(500);

        builder.Property(a => a.AdditionalData)
            .HasColumnType("text");

        builder.Property(a => a.Timestamp)
            .IsRequired()
            .HasColumnType("timestamp with time zone");

        // Índices para optimizar consultas
        builder.HasIndex(a => a.EventType);
        builder.HasIndex(a => a.EntityName);
        builder.HasIndex(a => a.EntityId);
        builder.HasIndex(a => a.Action);
        builder.HasIndex(a => a.UserId);
        builder.HasIndex(a => a.Timestamp);

        // Índice compuesto para consultas frecuentes
        builder.HasIndex(a => new { a.EventType, a.EntityId, a.Timestamp });
    }
}