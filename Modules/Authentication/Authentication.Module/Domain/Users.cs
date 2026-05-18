using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Authentication.Module.Domain;

public class Users
{
    [BsonId]
    [BsonElement("_id")]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; private set; }

    [BsonElement("email")]
    public string Email { get; private set; } = default!;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; private set; } = default!;

    [BsonElement("role")]
    public string Role { get; private set; } = default!;

    [BsonElement("firstName")]
    public string FirstName { get; private set; } = default!;

    [BsonElement("lastName")]
    public string LastName { get; private set; } = default!;

    [BsonElement("status")]
    public string Status { get; private set; } = default!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

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

    public void SetEmail(string newEmail)
    {
        Email = newEmail.Trim();
        MarkUpdated();
    }

    public void SetNewPassword(string newPasswordHash)
    {
        PasswordHash = newPasswordHash;
        MarkUpdated();
    }

    public void SetRole(string newRole)
    {
        Role = newRole.Trim().ToLowerInvariant();
        MarkUpdated();
    }

    public void SetName(string firstName, string lastName)
    {
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MarkUpdated();
    }

    public void SetStatusActive()
    {
        Status = "active";
        MarkUpdated();
    }

    public void SetStatusInactive()
    {
        Status = "inactive";
        MarkUpdated();
    }

    public void MarkUpdated()
    {
       UpdatedAt = DateTime.UtcNow;
    }

    public string FullName => $"{FirstName} {LastName}";
}
