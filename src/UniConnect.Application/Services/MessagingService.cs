using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class MessagingService : IMessagingService
{
    private readonly IUnitOfWork _unitOfWork;

    public MessagingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task SendMessageAsync(Guid senderId, Guid receiverId, string content)
    {
        var message = new DirectMessage
        {
            Id = Guid.NewGuid(),
            SenderId = senderId,
            ReceiverId = receiverId,
            Content = content,
            SentAtUtc = DateTime.UtcNow,
            IsRead = false
        };

        await _unitOfWork.DirectMessages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<DirectMessageDto>> GetConversationAsync(Guid userOneId, Guid userTwoId)
    {
        var messages = await _unitOfWork.DirectMessages.GetConversationAsync(userOneId, userTwoId);
        return messages.Select(m => new DirectMessageDto(m.Id, m.SenderId, m.ReceiverId, m.Content, m.SentAtUtc, m.IsRead));
    }
}