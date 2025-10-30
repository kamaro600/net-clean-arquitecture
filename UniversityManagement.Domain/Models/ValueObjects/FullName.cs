namespace UniversityManagement.Domain.Models.ValueObjects;

/// <summary>
/// Value Object que representa un nombre completo
/// </summary>
public class FullName : IEquatable<FullName>
{
    public string FirstName { get; }
    public string LastName { get; }
    public string FullDisplayName => $"{FirstName} {LastName}";

    public FullName(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(firstName));

        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
    }

    public override string ToString() => FullDisplayName;

    public bool Equals(FullName? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return FirstName == other.FirstName && LastName == other.LastName;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as FullName);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FirstName, LastName);
    }

    public static bool operator ==(FullName? left, FullName? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(FullName? left, FullName? right)
    {
        return !Equals(left, right);
    }
}