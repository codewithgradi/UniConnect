using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;

namespace UniConnect.Application.Services;

public interface IMessagingService
{
    Task<IEnumerable<DirectMessageDtoForAll>> GetAllMessages (Guid userId , CancellationToken ct);
    Task<DirectMessageDto> SendMessageAsync(Guid senderId, Guid receiverId, string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<DirectMessageDto>> GetConversationAsync(Guid userOneId, Guid userTwoId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkMessagesAsReadAsync(Guid userId, IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default);
}
public record DirectMessageDtoForAll(
    Guid Id,
    Guid SenderUserId,
    Guid SenderProfileId,
    string SenderFirstName,
    string SenderLastName,
    string message);