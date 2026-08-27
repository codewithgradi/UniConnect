using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Storage;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;
    private IDbContextTransaction? _currentTransaction;

    public IUserRepository Users { get; }
    public IUserProfileRepository UserProfiles { get; }
    public IBusinessProfileRepository BusinessProfiles { get; }
    public IPostRepository Posts { get; }
    public IConnectionRepository Connections { get; }
    public IDirectMessageRepository DirectMessages { get; }
    public IOpportunityRepository Opportunities { get; }
    public IInstitutionalEventRepository InstitutionalEvents { get; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        Users = new UserRepository(_dbContext);
        UserProfiles = new UserProfileRepository(_dbContext);
        BusinessProfiles = new BusinessProfileRepository(_dbContext);
        Posts = new PostRepository(_dbContext);
        Connections = new ConnectionRepository(_dbContext);
        DirectMessages = new DirectMessageRepository(_dbContext);
        Opportunities = new OpportunityRepository(_dbContext);
        InstitutionalEvents = new InstitutionalEventRepository(_dbContext);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null) return;
        _currentTransaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _dbContext.Dispose();
    }
}