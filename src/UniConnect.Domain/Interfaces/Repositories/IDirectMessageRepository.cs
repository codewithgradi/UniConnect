using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IDirectMessageRepository
{
    Task<IEnumerable<DirectMessage>> GetConversationAsync(Guid userOneId, Guid userTwoId, int skip = 0, int take = 50, CancellationToken cancellationToken = default);
    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(DirectMessage message, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default);
}