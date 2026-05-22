using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Patients.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class PatientDO
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("userId")]
    [BsonRepresentation(BsonType.String)]
    public Guid UserId { get; set; }

    [BsonElement("dateOfBirth")]
    public DateTime DateOfBirth { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = null!;

    [BsonElement("doctorId")]
    [BsonRepresentation(BsonType.String)]
    public Guid? DoctorId { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
