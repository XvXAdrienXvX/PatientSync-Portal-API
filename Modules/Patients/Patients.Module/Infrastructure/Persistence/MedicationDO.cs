using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Patients.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class MedicationDO
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("patientId")]
    [BsonRepresentation(BsonType.String)]
    public Guid PatientId { get; set; }

    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonElement("dosage")]
    public string Dosage { get; set; } = null!;

    [BsonElement("frequency")]
    public string Frequency { get; set; } = null!;

    [BsonElement("reason")]
    public string Reason { get; set; } = null!;

    [BsonElement("startedDate")]
    public DateTime StartedDate { get; set; }

    [BsonElement("endedDate")]
    public DateTime? EndedDate { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
