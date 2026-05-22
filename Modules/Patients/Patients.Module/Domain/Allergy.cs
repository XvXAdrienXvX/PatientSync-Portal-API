using System;

namespace Patients.Module.Domain;

public class Allergy
{
    public Guid Id { get; internal set; }
    public Guid PatientId { get; internal set; }
    public string Substance { get; internal set; } = null!;
    public string ReactionType { get; internal set; } = null!;
    public string Severity { get; internal set; } = null!;
    public DateTime LoggedAt { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    public static Allergy Create(
        Guid patientId,
        string substance,
        string reactionType,
        string severity,
        DateTime loggedAt)
    {
        var now = DateTime.UtcNow;
        return new Allergy
        {
            Id = Guid.NewGuid(),
            PatientId = patientId,
            Substance = substance.Trim(),
            ReactionType = reactionType.Trim(),
            Severity = severity.Trim(),
            LoggedAt = loggedAt,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    internal static Allergy Rehydrate(
        Guid id,
        Guid patientId,
        string substance,
        string reactionType,
        string severity,
        DateTime loggedAt,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Allergy
        {
            Id = id,
            PatientId = patientId,
            Substance = substance,
            ReactionType = reactionType,
            Severity = severity,
            LoggedAt = loggedAt,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void UpdateReaction(string reactionType, string severity)
    {
        ReactionType = reactionType.Trim();
        Severity = severity.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateSubstance(string substance)
    {
        Substance = substance.Trim();
        UpdatedAt = DateTime.UtcNow;
    }
}
