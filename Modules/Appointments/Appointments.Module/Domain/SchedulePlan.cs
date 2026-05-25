namespace Appointments.Module.Domain;

internal class SchedulePlan
{
    public Guid Id { get; internal set; }
    public Guid DoctorId { get; internal set; }
    public DateTime WeekStartDate { get; internal set; }
    public string Status { get; internal set; } = "draft";
    public List<Slot> Slots { get; internal set; } = [];
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    internal static SchedulePlan Create(
        Guid doctorId,
        DateTime weekStartDate,
        IEnumerable<(DateTime StartTime, int Duration)> slotRequests)
    {
        var now = DateTime.UtcNow;
        var plan = new SchedulePlan
        {
            Id = Guid.NewGuid(),
            DoctorId = doctorId,
            WeekStartDate = weekStartDate.Date,
            Status = "draft",
            CreatedAt = now,
            UpdatedAt = now
        };
        plan.Slots = slotRequests
            .Select(s => Slot.Create(plan.Id, doctorId, s.StartTime, s.Duration))
            .ToList();
        return plan;
    }

    internal static SchedulePlan Rehydrate(
        Guid id, Guid doctorId, DateTime weekStartDate,
        string status, List<Slot> slots, DateTime createdAt, DateTime updatedAt)
    {
        return new SchedulePlan
        {
            Id = id,
            DoctorId = doctorId,
            WeekStartDate = weekStartDate,
            Status = status,
            Slots = slots,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    internal void Publish()
    {
        if (Status == "published") return;
        Status = "published";
        UpdatedAt = DateTime.UtcNow;
    }

    internal Slot? FindSlot(Guid slotId) =>
        Slots.FirstOrDefault(s => s.Id == slotId);
}
