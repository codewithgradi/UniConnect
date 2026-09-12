using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace UniConnect.Infrastructure.Repositories;

public abstract class RepositoryBase<T> where T : class
{
    protected readonly ApplicationDbContext _dbContext;

    protected RepositoryBase(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
}