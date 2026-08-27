using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IMessagingService
{
    Task SendMessageAsync(Guid senderId, Guid receiverId, string content);
    Task<IEnumerable<DirectMessageDto>> GetConversationAsync(Guid userOneId, Guid userTwoId);
}