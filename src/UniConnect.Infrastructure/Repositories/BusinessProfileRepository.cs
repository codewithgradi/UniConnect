using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class BusinessProfileRepository : RepositoryBase<BusinessProfile>, IBusinessProfileRepository
{
    public BusinessProfileRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<BusinessProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BusinessProfiles
            .Include(b => b.Opportunities)
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
    }

    public async Task<BusinessProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BusinessProfiles
            .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);
    }

    public async Task<IEnumerable<BusinessProfile>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default)
    {
        return await _dbContext.BusinessProfiles
            .Where(b => b.Industry.ToLower() == industry.ToLower())
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(BusinessProfile businessProfile, CancellationToken cancellationToken = default)
    {
        
        await _dbContext.BusinessProfiles.AddAsync(businessProfile, cancellationToken);
    }

    public void Update(BusinessProfile businessProfile)
    {
        _dbContext.BusinessProfiles.Update(businessProfile);
    }
}