using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetUsersByTypeAsync(UserType userType, CancellationToken cancellationToken = default);
    Task<IEnumerable<ApplicationUser>> GetPendingVerificationsAsync(CancellationToken cancellationToken = default);
    Task UpdateStatusAsync(Guid userId, VerificationStatus status, CancellationToken cancellationToken = default);
}