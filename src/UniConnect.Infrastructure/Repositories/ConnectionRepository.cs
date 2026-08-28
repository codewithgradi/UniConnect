using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class ConnectionRepository : RepositoryBase<Connection>, IConnectionRepository
{
    public ConnectionRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Connection?> GetAsync(Guid requesterId, Guid receiverId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Connections
            .FirstOrDefaultAsync(c => (c.RequesterId == requesterId && c.ReceiverId == receiverId) ||
                                      (c.RequesterId == receiverId && c.ReceiverId == requesterId), cancellationToken);
    }

    public async Task<IEnumerable<Connection>> GetUserConnectionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Connections
            .Where(c => (c.RequesterId == userId || c.ReceiverId == userId) && c.Status == ConnectionStatus.Accepted) // 1 = Accepted
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Connection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Connections
            .Where(c => c.ReceiverId == userId && c.Status == ConnectionStatus.Pending) 
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Connection connection, CancellationToken cancellationToken = default)
    {
        await _dbContext.Connections.AddAsync(connection, cancellationToken);
    }

    public void Update(Connection connection)
    {
        _dbContext.Connections.Update(connection);
    }

    public void Remove(Connection connection)
    {
        _dbContext.Connections.Remove(connection);
    }
}