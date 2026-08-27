namespace UniConnect.Application.Services;

public interface IConnectionService
{
    Task SendConnectionRequestAsync(Guid requesterId, Guid receiverId);
    Task AcceptConnectionAsync(Guid requesterId, Guid receiverId);
}