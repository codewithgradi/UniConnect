using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class ConnectionService : IConnectionService
{
    private readonly IUnitOfWork _unitOfWork;

    public ConnectionService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task SendConnectionRequestAsync(Guid requesterUserId, Guid targetProfileId, CancellationToken cancellationToken = default)
    {
        // 1. Fetch target profile using ITS PRIMARY KEY (Id), NOT UserId
        var targetProfile = await _unitOfWork.UserProfiles.GetByUserProfileIdAsync(targetProfileId, cancellationToken);

        if (targetProfile == null)
        {
            throw new KeyNotFoundException($"UserProfile with ID '{targetProfileId}' was not found.");
        }

        // 2. Prevent sending a connection request to oneself
        if (requesterUserId == targetProfile.UserId)
        {
            throw new InvalidOperationException("You cannot send a connection request to yourself.");
        }

        // 3. Check if a connection already exists using both AspNetUser IDs
        var existingConnection = await _unitOfWork.Connections.GetAsync(requesterUserId, targetProfile.UserId, cancellationToken);
        if (existingConnection != null)
        {
            throw new InvalidOperationException("A connection request or active connection already exists.");
        }

        // 4. Create and persist the connection
        var connection = new Connection
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterUserId,
            ReceiverId = targetProfile.UserId, // Target's AspNetUsers ID retrieved from profile
            Status = ConnectionStatus.Pending,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Connections.AddAsync(connection, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
    public async Task AcceptConnectionAsync(Guid requesterId, Guid receiverId, CancellationToken cancellationToken = default)
    {
        var connection = await _unitOfWork.Connections.GetAsync(requesterId, receiverId, cancellationToken);
        if (connection == null || connection.ReceiverId != receiverId || connection.Status != ConnectionStatus.Pending)
        {
            throw new KeyNotFoundException("Pending connection request not found.");
        }

        connection.Status = ConnectionStatus.Accepted;
        _unitOfWork.Connections.Update(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectConnectionAsync(Guid requesterId, Guid receiverId, CancellationToken cancellationToken = default)
    {
        var connection = await _unitOfWork.Connections.GetAsync(requesterId, receiverId, cancellationToken);
        if (connection == null || connection.ReceiverId != receiverId || connection.Status != ConnectionStatus.Pending)
        {
            throw new KeyNotFoundException("Pending connection request not found.");
        }

        connection.Status = ConnectionStatus.Declined;
        _unitOfWork.Connections.Update(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveConnectionAsync(Guid userId, Guid targetUserId, CancellationToken cancellationToken = default)
    {
        var connection = await _unitOfWork.Connections.GetAsync(userId, targetUserId, cancellationToken);
        if (connection == null)
        {
            throw new KeyNotFoundException("Connection not found.");
        }

        _unitOfWork.Connections.Remove(connection);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Connection>> GetUserConnectionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Connections.GetUserConnectionsAsync(userId, cancellationToken);
    }

    public async Task<IEnumerable<Connection>> GetPendingRequestsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Connections.GetPendingRequestsAsync(userId, cancellationToken);
    }
}