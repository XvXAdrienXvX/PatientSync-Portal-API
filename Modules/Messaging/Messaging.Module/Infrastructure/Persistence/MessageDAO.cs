using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Messaging.Module.Infrastructure.Persistence;

[BsonIgnoreExtraElements]
public class MessageDAO
{
    [BsonId]
    [BsonRepresentation(BsonType.String)]
    public Guid Id { get; set; }

    [BsonElement("senderId")]
    [BsonRepresentation(BsonType.String)]
    public Guid SenderId { get; set; }

    [BsonElement("recipientId")]
    [BsonRepresentation(BsonType.String)]
    public Guid RecipientId { get; set; }

    [BsonElement("senderRole")]
    public string SenderRole { get; set; } = null!;

    [BsonElement("recipientRole")]
    public string RecipientRole { get; set; } = null!;

    [BsonElement("subject")]
    public string? Subject { get; set; }

    [BsonElement("content")]
    public string Content { get; set; } = null!;

    [BsonElement("status")]
    public string Status { get; set; } = null!;

    [BsonElement("readAt")]
    public DateTime? ReadAt { get; set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; }
}
