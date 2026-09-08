using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class DirectMessageRepository : RepositoryBase<DirectMessage>, IDirectMessageRepository
{
    public DirectMessageRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<IEnumerable<DirectMessage>> GetConversationAsync(Guid userOneId, Guid userTwoId, int skip = 0, int take = 50, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DirectMessages
            .Where(m => (m.SenderId == userOneId && m.ReceiverId == userTwoId) ||
                        (m.SenderId == userTwoId && m.ReceiverId == userOneId))
            .OrderByDescending(m => m.SentAtUtc)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
    }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.DirectMessages
            .CountAsync(m => m.ReceiverId == userId && !m.IsRead, cancellationToken);
    }

    public async Task AddAsync(DirectMessage message, CancellationToken cancellationToken = default)
    {
        await _dbContext.DirectMessages.AddAsync(message, cancellationToken);
    }

    public async Task MarkAsReadAsync(IEnumerable<Guid> messageIds, CancellationToken cancellationToken = default)
    {
        var messages = await _dbContext.DirectMessages
            .Where(m => messageIds.Contains(m.Id))
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            message.IsRead = true;
        }
    }

    public async Task<ICollection<DirectMessage>> GetAllMessages(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.DirectMessages.Where(x=>x.Receiver.Id == userId ).ToListAsync();
    }
}