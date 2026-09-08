using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IConnectionRepository
{
    Task<Connection?> GetAsync(Guid requesterId, Guid receiverId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Connection>> GetUserConnectionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Connection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Connection connection, CancellationToken cancellationToken = default);
    void Update(Connection connection);
    void Remove(Connection connection);
    Task<int> GetConnectionCount(Guid userId, CancellationToken cancellationToken);
}