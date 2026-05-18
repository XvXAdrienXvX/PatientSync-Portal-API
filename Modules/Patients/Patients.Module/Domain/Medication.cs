using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Patients.Module.Domain;

public class Medication
{
    [BsonId]
    public Guid Id { get; private set; }

    [BsonElement("patientId")]
    public Guid PatientId { get; private set; }

    [BsonElement("name")]
    public string Name { get; private set; } = null!;

    [BsonElement("dosage")]
    public string Dosage { get; private set; } = null!;

    [BsonElement("frequency")]
    public string Frequency { get; private set; } = null!;

    [BsonElement("reason")]
    public string Reason { get; private set; } = null!;

    [BsonElement("startedDate")]
    public DateTime StartedDate { get; private set; }

    [BsonElement("endedDate")]
    public DateTime? EndedDate { get; private set; }

    [BsonElement("status")]
    public string Status { get; private set; } = "active";

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    public static Medication Create(
        Guid patientId,
        string name,
        string dosage,
        string frequency,
        string reason,
        DateTime startedDate)
    {
        var now = DateTime.UtcNow;
        return new Medication
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Name = name.Trim(),
            Dosage = dosage.Trim(),
            Frequency = frequency.Trim(),
            Reason = reason.Trim(),
            StartedDate = startedDate,
            EndedDate = null,
            Status = "active",
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void UpdateDosage(string dosage)
    {
        Dosage = dosage.Trim();
        MarkAsUpdated();
    }

    public void UpdateFrequency(string frequency)
    {
        Frequency = frequency.Trim();
        MarkAsUpdated();
    }

    public void End(DateTime endedDate)
    {
        EndedDate = endedDate;
        Status = "inactive";
        MarkAsUpdated();
    }

    public void Reopen()
    {
        EndedDate = null;
        Status = "active";
        MarkAsUpdated();
    }

    private void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
