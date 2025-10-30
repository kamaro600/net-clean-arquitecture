namespace UniversityManagement.Domain.Models.ValueObjects;

/// <summary>
/// Value Object que representa una dirección completa
/// </summary>
public class Address : IEquatable<Address>
{
    public string Street { get; }
    public string? City { get; }
    public string? Province { get; }
    public string? PostalCode { get; }

    public Address(string street, string? city = null, string? province = null, string? postalCode = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("La dirección debe incluir al menos la calle.", nameof(street));

        Street = street.Trim();
        City = city?.Trim();
        Province = province?.Trim();
        PostalCode = postalCode?.Trim();
    }

    /// <summary>
    /// Constructor que acepta una dirección completa como string único
    /// </summary>
    public Address(string fullAddress)
    {
        if (string.IsNullOrWhiteSpace(fullAddress))
            throw new ArgumentException("La dirección no puede estar vacía.", nameof(fullAddress));

        // Parseamos la dirección completa como street por ahora
        // En una implementación más compleja, podríamos parsear las partes
        Street = fullAddress.Trim();
        City = null;
        Province = null;
        PostalCode = null;
    }

    public override string ToString()
    {
        var parts = new List<string> { Street };
        if (!string.IsNullOrEmpty(City)) parts.Add(City);
        if (!string.IsNullOrEmpty(Province)) parts.Add(Province);
        if (!string.IsNullOrEmpty(PostalCode)) parts.Add(PostalCode);
        
        return string.Join(", ", parts);
    }

    public bool Equals(Address? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Street == other.Street && 
               City == other.City && 
               Province == other.Province && 
               PostalCode == other.PostalCode;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Address);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, City, Province, PostalCode);
    }

    public static bool operator ==(Address? left, Address? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Address? left, Address? right)
    {
        return !Equals(left, right);
    }
}