using System;
using System.Text.RegularExpressions;

namespace NexusAdmin.Core.ValueObjects;

public class Email
{
    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.CultureInvariant | RegexOptions.Compiled | RegexOptions.IgnoreCase
    );

    public string Value { get; }

    private Email(string value)
    {
        this.Value = value;
    }

    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            throw new ArgumentException("Email cannot be empty", nameof(email));
        }

        email = email.Trim().ToLowerInvariant();

        if (!EmailRegex.IsMatch(email))
        {
            throw new ArgumentException($"Email '{email}' is invalid", nameof(email));
        }

        return new Email(email);
    }

    public override string ToString() => this.Value;

    public override bool Equals(object? obj)
    {
        if (obj is Email other)
        {
            return this.Value == other.Value;
        }

        return false;
    }

    public override int GetHashCode() => this.Value.GetHashCode();
}
