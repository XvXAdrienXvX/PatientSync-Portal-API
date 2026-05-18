using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Messaging.Module.Domain;

public class Message
{
    [BsonId]
    public ObjectId Id { get; private set; }

    [BsonElement("senderId")]
    public ObjectId SenderId { get; private set; }

    [BsonElement("recipientId")]
    public ObjectId RecipientId { get; private set; }

    [BsonElement("senderRole")]
    public string SenderRole { get; private set; } = null!;

    [BsonElement("recipientRole")]
    public string RecipientRole { get; private set; } = null!;

    [BsonElement("subject")]
    public string? Subject { get; private set; }

    [BsonElement("content")]
    public string Content { get; private set; } = null!;

    [BsonElement("status")]
    public string Status { get; private set; } = "unread";

    [BsonElement("readAt")]
    public DateTime? ReadAt { get; private set; }

    [BsonElement("isDeleted")]
    public bool IsDeleted { get; private set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; private set; }

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; private set; }

    public static Message Create(
        ObjectId senderId,
        ObjectId recipientId,
        string senderRole,
        string recipientRole,
        string content,
        string? subject = null)
    {
        var now = DateTime.UtcNow;
        return new Message
        {
            Id = ObjectId.GenerateNewId(),
            SenderId = senderId,
            RecipientId = recipientId,
            SenderRole = senderRole.Trim(),
            RecipientRole = recipientRole.Trim(),
            Content = content.Trim(),
            Subject = subject?.Trim(),
            Status = "unread",
            IsDeleted = false,
            CreatedAt = now,
            UpdatedAt = now
        };
    }

    public void MarkRead()
    {
        if (Status == "unread")
        {
            Status = "read";
            ReadAt = DateTime.UtcNow;
            Touch();
        }
    }

    public void Archive()
    {
        Status = "archived";
        Touch();
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        Touch();
    }

    public void Restore()
    {
        IsDeleted = false;
        Touch();
    }

    private void Touch()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
