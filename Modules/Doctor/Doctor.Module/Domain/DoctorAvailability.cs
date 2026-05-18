using MongoDB.Bson.Serialization.Attributes;

namespace Doctor.Module.Domain;

public class DoctorAvailability
{
    public DoctorAvailability(int dayOfWeek, string startTime, string endTime, int slotDuration)
    {
        DayOfWeek = dayOfWeek;
        StartTime = startTime.Trim();
        EndTime = endTime.Trim();
        SlotDuration = slotDuration;
    }

    [BsonElement("dayOfWeek")]
    public int DayOfWeek { get; private set; }

    [BsonElement("startTime")]
    public string StartTime { get; private set; } = null!;

    [BsonElement("endTime")]
    public string EndTime { get; private set; } = null!;

    [BsonElement("slotDuration")]
    public int SlotDuration { get; private set; }

    public void UpdateSchedule(string startTime, string endTime, int slotDuration)
    {
        StartTime = startTime.Trim();
        EndTime = endTime.Trim();
        SlotDuration = slotDuration;
    }
}
