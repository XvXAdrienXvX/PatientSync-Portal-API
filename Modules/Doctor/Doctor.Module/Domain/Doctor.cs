using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Doctor.Module.Domain;

public class Doctor
{
    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; private set; }

    [BsonElement("userId")]
    public Guid UserId { get; private set; }

    [BsonElement("specialization")]
    public string Specialization { get; private set; } = null!;

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
            Id = Guid.NewGuid(),
            UserId = userId,
            Specialization = specialization.Trim(),
            Availability = availability.Select(a => a).ToList(),
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void SetAvailability(IEnumerable<DoctorAvailability> availability)
    {
        Availability = availability.Select(a => a).ToList();
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetActive()
    {
        Status = "active";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetOnLeave()
    {
        Status = "onleave";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        Status = "inactive";
        UpdatedAt = DateTime.UtcNow;
    }
}
