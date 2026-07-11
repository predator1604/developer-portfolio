using System.Text.RegularExpressions;

namespace Portfolio.Domain.ValueObjects;

/// <summary>
/// Immutable Email value object. Validates format on creation.
/// Equality is value-based (two Emails with the same address are equal).
/// </summary>
public sealed class Email : IEquatable<Email>
{
    private static readonly Regex _regex = new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    public string Value { get; }

    private Email(string value) => Value = value.ToLowerInvariant().Trim();

    public static Email Create(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if (!_regex.IsMatch(value))
            throw new ArgumentException($"'{value}' is not a valid email address.", nameof(value));

        return new Email(value);
    }

    public override string ToString() => Value;

    public bool Equals(Email? other) => other is not null && Value == other.Value;

    public override bool Equals(object? obj) => obj is Email other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(Value);

    public static implicit operator string(Email email) => email.Value;
}
