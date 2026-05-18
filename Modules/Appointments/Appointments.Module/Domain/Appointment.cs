using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Module.Domain;

public class Appointment
{
    [BsonId]
    [BsonElement("_id")]
    public Guid Id { get; private set; }

    [BsonElement("patientId")]
    public Guid PatientId { get; private set; }

    [BsonElement("doctorId")]
    public Guid DoctorId { get; private set; }

    [BsonElement("scheduledAt")]
    public DateTime ScheduledAt { get; private set; }

    [BsonElement("duration")]
    public int Duration { get; private set; }

    [BsonElement("chiefComplaint")]
    public string ChiefComplaint { get; private set; } = null!;

    [BsonElement("status")]
    public string Status { get; private set; } = "scheduled";

    [BsonElement("visitNotes")]
    public VisitNotes? VisitNotes { get; private set; }

    [BsonElement("cancelledAt")]
    public DateTime? CancelledAt { get; private set; }

    [BsonElement("cancelledBy")]
    public string? CancelledBy { get; private set; }

    [BsonElement("cancellationReason")]
    public string? CancellationReason { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    public static Appointment Schedule(
        Guid patientId,
        Guid doctorId,
        DateTime scheduledAt,
        int duration,
        string chiefComplaint)
    {
        var now = DateTime.UtcNow;
        return new Appointment
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            DoctorId = doctorId,
            ScheduledAt = scheduledAt,
            Duration = duration,
            ChiefComplaint = chiefComplaint.Trim(),
            Status = "scheduled",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void Reschedule(DateTime newScheduledAt)
    {
        if (Status != "scheduled")
        {
            throw new InvalidOperationException("Only scheduled appointments can be rescheduled.");
        }

        ScheduledAt = newScheduledAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string cancelledBy, string cancellationReason)
    {
        if (Status == "cancelled")
        {
            return;
        }

        Status = "cancelled";
        CancelledAt = DateTime.UtcNow;
        CancelledBy = cancelledBy.Trim();
        CancellationReason = cancellationReason.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete(string assessment, string plan, DateTime? deniedAt = null)
    {
        if (Status == "cancelled")
        {
            throw new InvalidOperationException("A cancelled appointment cannot be completed.");
        }

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
