using System;

namespace Patients.Module.Domain;

public class Medication
{
    public Guid Id { get; internal set; }
    public Guid PatientId { get; internal set; }
    public string Name { get; internal set; } = null!;
    public string Dosage { get; internal set; } = null!;
    public string Frequency { get; internal set; } = null!;
    public string Reason { get; internal set; } = null!;
    public DateTime StartedDate { get; internal set; }
    public DateTime? EndedDate { get; internal set; }
    public string Status { get; internal set; } = "active";
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

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

    internal static Medication Rehydrate(
        Guid id,
        Guid patientId,
        string name,
        string dosage,
        string frequency,
        string reason,
        DateTime startedDate,
        DateTime? endedDate,
        string status,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Medication
        {
            Id = id,
            PatientId = patientId,
            Name = name,
            Dosage = dosage,
            Frequency = frequency,
            Reason = reason,
            StartedDate = startedDate,
            EndedDate = endedDate,
            Status = status,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void UpdateDosage(string dosage)
    {
        Dosage = dosage.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFrequency(string frequency)
    {
        Frequency = frequency.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void End(DateTime endedDate)
    {
        EndedDate = endedDate;
        Status = "inactive";
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reopen()
    {
        EndedDate = null;
        Status = "active";
        UpdatedAt = DateTime.UtcNow;
    }
}
