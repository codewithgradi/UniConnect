using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class OpportunityRepository : RepositoryBase<Opportunity>, IOpportunityRepository
{
    public OpportunityRepository(ApplicationDbContext dbContext) : base(dbContext) { }
    public async Task<IEnumerable<Opportunity>> GetPendingOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Opportunities
            .Where(o => o.Status == OpportunityStatus.PendingApproval)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Opportunity>> GetByBusinessProfileIdAsync(Guid businessProfileId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Opportunities
            .Where(o => o.BusinessProfileId == businessProfileId)
            .OrderByDescending(o => o.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }
    public async Task<Opportunity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Opportunities
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Opportunity?> GetWithApplicationsAsync(Guid opportunityId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Opportunities
            .Include(o => o.Applications)
            .FirstOrDefaultAsync(o => o.Id == opportunityId, cancellationToken);
    }

    public async Task<IEnumerable<Opportunity>> GetActiveOpportunitiesAsync(string? targetProgramme, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Opportunities.Where(o => o.Status == OpportunityStatus.Published);

        if (!string.IsNullOrWhiteSpace(targetProgramme))
        {
            query = query.Where(o => o.TargetProgramme != null && o.TargetProgramme.ToLower().Contains(targetProgramme.ToLower()));
        }

        return await query.OrderByDescending(o => o.CreatedAtUtc).ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Opportunity opportunity, CancellationToken cancellationToken = default)
    {
        await _dbContext.Opportunities.AddAsync(opportunity, cancellationToken);
    }

    public async Task AddApplicationAsync(JobApplication application, CancellationToken cancellationToken = default)
    {
        await _dbContext.JobApplications.AddAsync(application, cancellationToken);
    }

    public async Task<bool> HasUserAppliedAsync(Guid opportunityId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.JobApplications
            .AnyAsync(a => a.OpportunityId == opportunityId && a.ApplicantId == userId, cancellationToken);
    }
}