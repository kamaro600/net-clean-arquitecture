using System.Text.RegularExpressions;

namespace UniversityManagement.Domain.Models.ValueObjects;

/// <summary>
/// Value Object que representa un número de teléfono válido
/// </summary>
public class Phone : IEquatable<Phone>
{
    private static readonly Regex PhoneRegex = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);

    public string Value { get; }

    public Phone(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("El teléfono no puede estar vacío.", nameof(value));

        // Limpiar el número (remover espacios, guiones, etc.)
        var cleanValue = CleanPhoneNumber(value);

        if (!PhoneRegex.IsMatch(cleanValue))
            throw new ArgumentException($"El teléfono '{value}' no tiene un formato válido.", nameof(value));

        Value = cleanValue;
    }

    private static string CleanPhoneNumber(string phone)
    {
        return Regex.Replace(phone, @"[\s\-\(\)]", "");
    }

    public static implicit operator string(Phone phone) => phone.Value;
    public static implicit operator Phone(string phone) => new(phone);

    public override string ToString() => Value;

    public bool Equals(Phone? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Phone);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(Phone? left, Phone? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Phone? left, Phone? right)
    {
        return !Equals(left, right);
    }
}