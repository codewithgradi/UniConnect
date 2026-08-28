using UniConnect.Domain.Entities;

namespace UniConnect.Application.Services;

public interface IConnectionService
{
    Task SendConnectionRequestAsync(Guid requesterId, Guid receiverId, CancellationToken cancellationToken = default);
    Task AcceptConnectionAsync(Guid requesterId, Guid receiverId, CancellationToken cancellationToken = default);
    Task RejectConnectionAsync(Guid requesterId, Guid receiverId, CancellationToken cancellationToken = default);
    Task RemoveConnectionAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Connection>> GetUserConnectionsAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Connection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken = default);
}