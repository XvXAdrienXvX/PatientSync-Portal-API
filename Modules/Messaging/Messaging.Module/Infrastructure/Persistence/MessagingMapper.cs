using Messaging.Module.Domain;

namespace Messaging.Module.Infrastructure.Persistence;

public static class MessagingMapper
{
    public static MessageDO ToDataObject(Message domain)
    {
        return new MessageDO
        {
            Id = domain.Id,
            SenderId = domain.SenderId,
            RecipientId = domain.RecipientId,
            SenderRole = domain.SenderRole,
            RecipientRole = domain.RecipientRole,
            Subject = domain.Subject,
            Content = domain.Content,
            Status = domain.Status,
            ReadAt = domain.ReadAt,
            IsDeleted = domain.IsDeleted,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Message ToDomain(MessageDO data)
    {
        return Message.Rehydrate(
            data.Id,
            data.SenderId,
            data.RecipientId,
            data.SenderRole,
            data.RecipientRole,
            data.Subject,
            data.Content,
            data.Status,
            data.ReadAt,
            data.IsDeleted,
            data.CreatedAt,
            data.UpdatedAt);
    }
}
