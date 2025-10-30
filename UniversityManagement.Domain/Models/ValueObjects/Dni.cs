using System.Text.RegularExpressions;
using UniversityManagement.Domain.Exceptions;

namespace UniversityManagement.Domain.Models.ValueObjects;

/// <summary>
/// Value Object que representa un DNI válido
/// </summary>
public class Dni : IEquatable<Dni>
{
    private static readonly Regex DniRegex = new(@"^\d{8}[A-Za-z]?$", RegexOptions.Compiled);

    public string Value { get; }

    public Dni(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El DNI no puede estar vacío.", nameof(value));

        if (!DniRegex.IsMatch(value))
            throw new InvalidDniException(value);

        Value = value.ToUpperInvariant();
    }

    public static implicit operator string(Dni dni) => dni.Value;
    public static implicit operator Dni(string dni) => new(dni);

    public override string ToString() => Value;

    public bool Equals(Dni? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Dni);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(Dni? left, Dni? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Dni? left, Dni? right)
    {
        return !Equals(left, right);
    }
}