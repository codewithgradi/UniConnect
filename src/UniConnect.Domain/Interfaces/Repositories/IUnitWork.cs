namespace UniConnect.Domain.Interfaces.Repositories;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IUserProfileRepository UserProfiles { get; }
    IBusinessProfileRepository BusinessProfiles { get; }
    IPostRepository Posts { get; }
    IConnectionRepository Connections { get; }
    IDirectMessageRepository DirectMessages { get; }
    IOpportunityRepository Opportunities { get; }
    IInstitutionalEventRepository InstitutionalEvents { get; }
    ISkillRepository Skills{ get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}