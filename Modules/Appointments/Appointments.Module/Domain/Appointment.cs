namespace Appointments.Module.Domain;

public class Appointment
{
    public Guid Id { get; internal set; }
    public Guid PatientId { get; internal set; }
    public Guid DoctorId { get; internal set; }
    public DateTime ScheduledAt { get; internal set; }
    public int Duration { get; internal set; }
    public string PatientComplaint { get; internal set; } = null!;
    public string Status { get; internal set; } = "scheduled";
    public VisitNotes? VisitNotes { get; internal set; }
    public DateTime? CancelledAt { get; internal set; }
    public string? CancelledBy { get; internal set; }
    public string? CancellationReason { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    public static Appointment Schedule(
        Guid patientId,
        Guid doctorId,
        DateTime scheduledAt,
        int duration,
        string patientComplaint,
        Guid? id = null)
    {
        var now = DateTime.UtcNow;
        return new Appointment
        {
            Id = id ?? Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            ScheduledAt = scheduledAt,
            Duration = duration,
            PatientComplaint = patientComplaint.Trim(),
            Status = "scheduled",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    internal static Appointment Rehydrate(
        Guid id,
        Guid patientId,
        Guid doctorId,
        DateTime scheduledAt,
        int duration,
        string patientComplaint,
        string status,
        VisitNotes? visitNotes,
        DateTime? cancelledAt,
        string? cancelledBy,
        string? cancellationReason,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Appointment
        {
            Id = id,
            PatientId = patientId,
            DoctorId = doctorId,
            ScheduledAt = scheduledAt,
            Duration = duration,
            PatientComplaint = patientComplaint,
            Status = status,
            VisitNotes = visitNotes,
            CancelledAt = cancelledAt,
            CancelledBy = cancelledBy,
            CancellationReason = cancellationReason,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void Reschedule(DateTime newScheduledAt)
    {
        if (Status != "scheduled")
            throw new InvalidOperationException("Only scheduled appointments can be rescheduled.");

        ScheduledAt = newScheduledAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string cancelledBy, string cancellationReason)
    {
        if (Status == "cancelled") return;

        Status = "cancelled";
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy.Trim();
        CancellationReason = cancellationReason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string assessment, string plan, DateTime? deniedAt = null)
    {
        if (Status == "cancelled")
            throw new InvalidOperationException("A cancelled appointment cannot be completed.");

        Status = "completed";
        VisitNotes = new VisitNotes(assessment.Trim(), plan.Trim(), deniedAt);
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddOrUpdateVisitNotes(string assessment, string plan, DateTime? deniedAt = null)
    {
        VisitNotes = new VisitNotes(assessment.Trim(), plan.Trim(), deniedAt);
        UpdatedAt = DateTime.UtcNow;
    }
}
