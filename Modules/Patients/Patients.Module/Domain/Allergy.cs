using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Patients.Module.Domain;

public class Allergy
{
    [BsonId]
    public Guid Id { get; private set; }

    [BsonElement("patientId")]
    public Guid PatientId { get; private set; }

    [BsonElement("substance")]
    public string Substance { get; private set; } = null!;

    [BsonElement("reactionType")]
    public string ReactionType { get; private set; } = null!;

    [BsonElement("severity")]
    public string Severity { get; private set; } = null!;

    [BsonElement("loggedAt")]
    public DateTime LoggedAt { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

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

    public void UpdateReaction(string reactionType, string severity)
    {
        ReactionType = reactionType.Trim();
        Severity = severity.Trim();
        MarkAsUpdated();
    }

    public void UpdateSubstance(string substance)
    {
        Substance = substance.Trim();
        MarkAsUpdated();
    }

    private void MarkAsUpdated()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
