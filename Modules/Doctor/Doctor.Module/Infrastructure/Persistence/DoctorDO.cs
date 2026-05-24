using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Doctor.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class DoctorDO
{
    [BsonId]
    public Guid Id { get; set; }

    [BsonElement("userId")]
    public Guid UserId { get; set; }

    [BsonElement("specialization")]
    public string Specialization { get; set; } = null!;

    [BsonElement("licenseNumber")]
    public string LicenseNumber { get; set; } = null!;

    [BsonElement("availability")]
    public List<DoctorAvailabilityDO> Availability { get; set; } = new();

    [BsonElement("status")]
    public string Status { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
