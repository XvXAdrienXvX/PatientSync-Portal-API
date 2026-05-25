using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
internal class SlotDO
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("schedulePlanId")]
    [BsonRepresentation(BsonType.String)]
    public Guid SchedulePlanId { get; set; }

    [BsonElement("doctorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid DoctorId { get; set; }

    [BsonElement("startTime")]
    public DateTime StartTime { get; set; }

    [BsonElement("duration")]
    public int Duration { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = "available";

    [BsonElement("appointmentId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? AppointmentId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
