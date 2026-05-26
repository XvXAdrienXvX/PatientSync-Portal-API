using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Doctor.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
internal class SchedulePlanDO
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("doctorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid DoctorId { get; set; }

    [BsonElement("weekStartDate")]
    public DateTime WeekStartDate { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "draft";

    [BsonElement("slots")]
    public List<SlotDO> Slots { get; set; } = [];

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
