using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class VisitNotesDO
{
    [BsonElement("assessment")]
    public string Assessment { get; set; } = null!;

    [BsonElement("plan")]
    public string Plan { get; set; } = null!;

    [BsonElement("deniedAt")]
    public DateTime? DeniedAt { get; set; }
}
