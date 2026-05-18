using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Module.Domain;

public class VisitNotes
{
    public VisitNotes(string assessment, string plan, DateTime? deniedAt)
    {
        Assessment = assessment;
        Plan = plan;
        DeniedAt = deniedAt;
    }

    [BsonElement("assessment")]
    public string Assessment { get; private set; }

    [BsonElement("plan")]
    public string Plan { get; private set; }

    [BsonElement("deniedAt")]
    public DateTime? DeniedAt { get; private set; }
}
