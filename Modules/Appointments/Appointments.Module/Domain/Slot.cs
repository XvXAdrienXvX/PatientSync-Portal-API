namespace Appointments.Module.Domain;

internal class Slot
{
    public Guid Id { get; internal set; }
    public Guid SchedulePlanId { get; internal set; }
    public Guid DoctorId { get; internal set; }
    public DateTime StartTime { get; internal set; }
    public int Duration { get; internal set; }
    public string Status { get; internal set; } = "available";
    public Guid? AppointmentId { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    internal static Slot Create(Guid schedulePlanId, Guid doctorId, DateTime startTime, int duration)
    {
        var now = DateTime.UtcNow;
        return new Slot
        {
            Id = Guid.NewGuid(),
            SchedulePlanId = schedulePlanId,
            DoctorId = doctorId,
            StartTime = startTime,
            Duration = duration,
            Status = "available",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    internal static Slot Rehydrate(
        Guid id, Guid schedulePlanId, Guid doctorId,
        DateTime startTime, int duration, string status,
        Guid? appointmentId, DateTime createdAt, DateTime updatedAt)
    {
        return new Slot
        {
            Id = id,
            SchedulePlanId = schedulePlanId,
            DoctorId = doctorId,
            StartTime = startTime,
            Duration = duration,
            Status = status,
            AppointmentId = appointmentId,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    internal void Book(Guid appointmentId)
    {
        if (Status == "booked")
            throw new InvalidOperationException("Slot is already booked.");
        Status = "booked";
        AppointmentId = appointmentId;
        UpdatedAt = DateTime.UtcNow;
    }
}
