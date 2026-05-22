using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Appointments.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class LabOrderDO
{
    [BsonId]
    public Guid OrderId { get; set; }

    [BsonElement("patientId")]
    public Guid PatientId { get; set; }

    [BsonElement("doctorId")]
    public Guid DoctorId { get; set; }

    [BsonElement("testType")]
    public string TestType { get; set; } = null!;

    [BsonElement("orderedDate")]
    public DateTime OrderedDate { get; set; }

    [BsonElement("status")]
    public string Status { get; set; } = null!;

    [BsonElement("resultsPdfUrl")]
    public string? ResultsPdfUrl { get; set; }

    [BsonElement("doctorNotes")]
    public string? DoctorNotes { get; set; }

    [BsonElement("resultsReceivedDate")]
    public DateTime? ResultsReceivedDate { get; set; }
}
