using MongoDB.Bson.Serialization.Attributes;

namespace Doctor.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class DoctorAvailabilityDO
{
    [BsonElement("dayOfWeek")]
    public int DayOfWeek { get; set; }

    [BsonElement("startTime")]
    public string StartTime { get; set; } = null!;

    [BsonElement("endTime")]
    public string EndTime { get; set; } = null!;

    [BsonElement("slotDuration")]
    public int SlotDuration { get; set; }
}
