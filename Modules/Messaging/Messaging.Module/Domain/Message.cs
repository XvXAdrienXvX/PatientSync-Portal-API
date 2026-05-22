using System;

namespace Messaging.Module.Domain;

public class Message
{
    public Guid Id { get; internal set; }
    public Guid SenderId { get; internal set; }
    public Guid RecipientId { get; internal set; }
    public string SenderRole { get; internal set; } = null!;
    public string RecipientRole { get; internal set; } = null!;
    public string? Subject { get; internal set; }
    public string Content { get; internal set; } = null!;
    public string Status { get; internal set; } = "unread";
    public DateTime? ReadAt { get; internal set; }
    public bool IsDeleted { get; internal set; }
    public DateTime CreatedAt { get; internal set; }
    public DateTime UpdatedAt { get; internal set; }

    public static Message Create(
        Guid senderId,
        Guid recipientId,
        string senderRole,
        string recipientRole,
        string content,
        string? subject = null)
    {
        var now = DateTime.UtcNow;
        return new Message
        {
            Id = Guid.NewGuid(),
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

    internal static Message Rehydrate(
        Guid id,
        Guid senderId,
        Guid recipientId,
        string senderRole,
        string recipientRole,
        string? subject,
        string content,
        string status,
        DateTime? readAt,
        bool isDeleted,
        DateTime createdAt,
        DateTime updatedAt)
    {
        return new Message
        {
            Id = id,
            SenderId = senderId,
            RecipientId = recipientId,
            SenderRole = senderRole,
            RecipientRole = recipientRole,
            Subject = subject,
            Content = content,
            Status = status,
            ReadAt = readAt,
            IsDeleted = isDeleted,
            CreatedAt = createdAt,
            UpdatedAt = updatedAt
        };
    }

    public void MarkRead()
    {
        if (Status == "unread")
        {
            Status = "read";
            ReadAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Archive()
    {
        Status = "archived";
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Restore()
    {
        IsDeleted = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
