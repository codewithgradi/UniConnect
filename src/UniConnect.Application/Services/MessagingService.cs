using UniConnect.Application.DTOs;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class MessagingService : IMessagingService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<DirectMessageDto> SendMessageAsync(Guid senderId, Guid receiverId, string content, CancellationToken cancellationToken = default)
    {
        if (senderId == receiverId)
        {
            throw new InvalidOperationException("You cannot send a message to yourself.");
        }

        if (string.IsNullOrWhiteSpace(content))
        {
            throw new ArgumentException("Message content cannot be empty.", nameof(content));
        }

        // Resolve sender: check if the incoming senderId is a UserProfile ID and map to the underlying User/Account ID
        var senderProfile = await _unitOfWork.UserProfiles
            .GetByUserProfileIdAsync(senderId , cancellationToken);

        var resolvedSenderId = senderProfile?.UserId ?? senderId;

        // Resolve receiver: check if the incoming receiverId (from the profile route) is a UserProfile ID and map to the underlying User/Account ID
        var receiverProfile = await _unitOfWork.UserProfiles
            .GetByUserProfileIdAsync( receiverId , cancellationToken);

        var resolvedReceiverId = receiverProfile?.UserId ?? receiverId;

        var message = new DirectMessage
        {
            Id = Guid.NewGuid(),
            SenderId = resolvedSenderId,
            ReceiverId = resolvedReceiverId,
            Content = content.Trim(),
            SentAtUtc = DateTime.UtcNow,
            IsRead = false
        };

        await _unitOfWork.DirectMessages.AddAsync(message, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new DirectMessageDto(
            message.Id,
            message.SenderId,
            message.ReceiverId,
            message.Content,
            message.SentAtUtc,
            message.IsRead
        );
    }
    public async Task<IEnumerable<DirectMessageDto>> GetConversationAsync(Guid userOneId, Guid userTwoId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        var messages = await _unitOfWork.DirectMessages.GetConversationAsync(userOneId, userTwoId, skip, take, cancellationToken);

        return messages
            .OrderBy(m => m.SentAtUtc) // Order chronologically for conversation view
            .Select(m => new DirectMessageDto(
                m.Id,
                m.SenderId,
                m.ReceiverId,
                m.Content,
                m.SentAtUtc,
                m.IsRead
            ));
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.DirectMessages.GetUnreadCountAsync(userId, cancellationToken);
    }

    public async Task MarkMessagesAsReadAsync(Guid userId, IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default)
    {
        var idsToUpdate = messageIds?.ToList();
        if (idsToUpdate == null || !idsToUpdate.Any()) return;

        await _unitOfWork.DirectMessages.MarkAsReadAsync(idsToUpdate, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<DirectMessageDtoForAll>> GetAllMessages(Guid userId, CancellationToken ct)
    {
        var messages = await _unitOfWork.DirectMessages.GetAllMessages(userId, ct);

        return messages
        .GroupBy(m => m.SenderId)
        .Select(g => g.OrderByDescending(m => m.SentAtUtc).FirstOrDefault())
        .Where(m => m != null)
        .Select(m => new DirectMessageDtoForAll(
            m.Id,
            m.SenderId,
            m.Sender?.Profile?.Id ?? Guid.Empty,
            m.Sender?.Profile?.FirstName ?? string.Empty,
            m.Sender?.Profile?.LastName ?? string.Empty,
            m.Content
        ));
    }
}