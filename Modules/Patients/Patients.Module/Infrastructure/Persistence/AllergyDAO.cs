using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Patients.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class AllergyDAO
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("patientId")]
    [BsonRepresentation(BsonType.String)]
    public Guid PatientId { get; set; }

    [BsonElement("substance")]
    public string Substance { get; set; } = null!;

    [BsonElement("reactionType")]
    public string ReactionType { get; set; } = null!;

    [BsonElement("severity")]
    public string Severity { get; set; } = null!;

    [BsonElement("loggedAt")]
    public DateTime LoggedAt { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
