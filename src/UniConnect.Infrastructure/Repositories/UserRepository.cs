using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class UserRepository : RepositoryBase<ApplicationUser>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task<IEnumerable<ApplicationUser>> GetUsersByTypeAsync(UserType userType, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.UserType == userType)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ApplicationUser>> GetPendingVerificationsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => u.VerificationStatus == VerificationStatus.Pending)
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateStatusAsync(Guid userId, VerificationStatus status, CancellationToken cancellationToken = default)
    {
        var user = await _dbContext.Users.FindAsync(new object[] { userId }, cancellationToken);
        if (user != null)
        {
            user.VerificationStatus = status;
        }
    }
}