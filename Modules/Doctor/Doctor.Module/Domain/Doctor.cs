using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Doctor.Module.Domain;

public class Doctor
{
    [BsonId]
    public ObjectId Id { get; private set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; private set; }

    [BsonElement("email")]
    public string Email { get; private set; } = null!;

    [BsonElement("firstName")]
    public string FirstName { get; private set; } = null!;

    [BsonElement("lastName")]
    public string LastName { get; private set; } = default!;

    [BsonElement("specialization")]
    public string Specialization { get; private set; } = null!;

    [BsonElement("licenseNumber")]
    public string LicenseNumber { get; private set; } = null!;

    [BsonElement("availability")]
    public List<DoctorAvailability> Availability { get; private set; } = new();

    [BsonElement("status")]
    public string Status { get; private set; } = "active";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    public static Doctor Create(
        Guid userId,
        string email,
        string firstName,
        string lastName,
        string specialization,
        string licenseNumber,
        IEnumerable<DoctorAvailability> availability)
    {
        var now = DateTime.UtcNow;
        return new Doctor
        {
            Id = ObjectId.GenerateNewId(),
            UserId = userId,
            Email = email.Trim(),
            FirstName = firstName.Trim(),
            LastName = lastName.Trim(),
            Specialization = specialization.Trim(),
            LicenseNumber = licenseNumber.Trim(),
            Availability = availability.Select(a => a).ToList(),
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateProfile(string email, string firstName, string lastName, string specialization)
    {
        Email = email.Trim();
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Specialization = specialization.Trim();
        Touch();
    }

    public void SetLicenseNumber(string licenseNumber)
    {
        LicenseNumber = licenseNumber.Trim();
        Touch();
    }

    public void SetAvailability(IEnumerable<DoctorAvailability> availability)
    {
        Availability = availability.Select(a => a).ToList();
        Touch();
    }

    public void Activate()
    {
        Status = "active";
        Touch();
    }

    public void SetOnLeave()
    {
        Status = "on-leave";
        Touch();
    }

    public void Deactivate()
    {
        Status = "inactive";
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
