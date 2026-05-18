using System;

namespace Authentication.Module.Domain;

public class Users
{
    public Guid Id { get; internal set; }
    public string Email { get; internal set; } = default!;
    public string PasswordHash { get; internal set; } = default!;
    public string Role { get; internal set; } = default!;
    public string FirstName { get; internal set; } = default!;
    public string LastName { get; internal set; } = default!;
    public string Status { get; internal set; } = default!;
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    public static Users Create(
        Guid id,
        string email,
        string passwordHash,
        string role,
        string firstName,
        string lastName,
        string status = "active")
    {
        var now = DateTime.UtcNow;
        return new Users
        {
            Id = id,
            Email = email.Trim(),
            PasswordHash = passwordHash,
            Role = role.Trim().ToLowerInvariant(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Status = status.Trim().ToLowerInvariant(),
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    internal static Users Rehydrate(
        Guid id,
        string email,
        string passwordHash,
        string role,
        string firstName,
        string lastName,
        string status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Users
        {
            Id = id,
            Email = email,
            PasswordHash = passwordHash,
            Role = role,
            FirstName = firstName,
            LastName = lastName,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void SetEmail(string newEmail)
    {
        Email = newEmail.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetNewPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetRole(string newRole)
    {
        Role = newRole.Trim().ToLowerInvariant();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetName(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatusActive()
    {
        Status = "active";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatusInactive()
    {
        Status = "inactive";
        UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";
}
