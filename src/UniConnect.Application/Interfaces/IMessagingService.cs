using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IMessagingService
{
    Task<DirectMessageDto> SendMessageAsync(Guid senderId, Guid receiverId, string content, CancellationToken cancellationToken = default);
    Task<IEnumerable<DirectMessageDto>> GetConversationAsync(Guid userOneId, Guid userTwoId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task MarkMessagesAsReadAsync(Guid userId, IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default);
}