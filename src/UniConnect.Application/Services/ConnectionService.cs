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

    public async Task SendConnectionRequestAsync(Guid requesterId, Guid receiverId)
    {
        var connection = new Connection
        {
            Id = Guid.NewGuid(),
            RequesterId = requesterId,
            ReceiverId = receiverId,
            Status = 0, // Pending
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Connections.AddAsync(connection);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AcceptConnectionAsync(Guid requesterId, Guid receiverId)
    {
        var connection = await _unitOfWork.Connections.GetAsync(requesterId, receiverId);
        if (connection == null) throw new KeyNotFoundException("Connection request not found.");

        connection.Status = ConnectionStatus.Accepted; // Accepted
        _unitOfWork.Connections.Update(connection);
        await _unitOfWork.SaveChangesAsync();
    }
}