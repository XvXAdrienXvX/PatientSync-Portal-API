using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Patients.Module.Domain;

public class Patient
{
    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; private set; }

    [BsonElement("userId")]
    public Guid UserId { get; private set; }

    [BsonElement("dateOfBirth")]
    public DateTime DateOfBirth { get; private set; }

    [BsonElement("status")]
    public string Status { get; private set; } = "active";

    [BsonElement("doctorId")]
    public Guid? DoctorId { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    public static Patient Create(
        Guid id,
        Guid userId,
        DateTime dateOfBirth,
        string? phone,
        Guid? doctorId)
    {
        var now = DateTime.UtcNow;
        return new Patient
        {
            Id = id,
            UserId = userId,
            DateOfBirth = dateOfBirth,
            DoctorId = doctorId,
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}
