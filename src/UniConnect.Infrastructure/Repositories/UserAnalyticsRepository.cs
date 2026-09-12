using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Application.DTOs;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class UserAnalyticsRepository : IUserAnalyticsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserAnalyticsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudentAnalyticsDto> GetStudentAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var appliedJobs = await _dbContext.JobApplications
            .CountAsync(a => a.ApplicantId == userId, cancellationToken);

        var totalConnections = await _dbContext.Connections
            .CountAsync(c => (c.RequesterId == userId || c.ReceiverId == userId) && c.Status == ConnectionStatus.Accepted, cancellationToken);

        var pendingConnections = await _dbContext.Connections
            .CountAsync(c => c.ReceiverId == userId && c.Status == ConnectionStatus.Pending, cancellationToken);

        // Calculate total skill endorsements received across all user skills
        var totalEndorsements = await _dbContext.UserSkills
            .Where(s => s.UserProfileId == userId)
            .SumAsync(s => s.Endorsements.Count, cancellationToken);

        var recentApplications = await _dbContext.JobApplications
            .Where(a => a.ApplicantId == userId)
            .OrderByDescending(a => a.AppliedAtUtc)
            .Take(5)
            .Select(a => new RecentApplicationStatusDto(
                a.Id,
                a.Opportunity.Title,
                a.Opportunity.BusinessProfile.CompanyName,
                a.Opportunity.Status.ToString(),
                a.AppliedAtUtc
            ))
            .ToListAsync(cancellationToken);

        return new StudentAnalyticsDto(
            StudentId: userId,
            AppliedJobsCount: appliedJobs,
            BookmarkedJobsCount: 0, // Extendable with Bookmarks table
            TotalConnections: totalConnections,
            PendingConnectionRequests: pendingConnections,
            TotalEndorsementsReceived: totalEndorsements,
            ProfileViewsCount: 0, // Extendable with ProfileViews table
            RecentApplications: recentApplications
        );
    }

    public async Task<BusinessAnalyticsDto> GetBusinessAnalyticsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var businessProfile = await _dbContext.BusinessProfiles
            .FirstOrDefaultAsync(b => b.UserId == userId, cancellationToken);

        if (businessProfile == null)
        {
            throw new KeyNotFoundException("Business profile not found for the given user.");
        }

        var businessId = businessProfile.Id;

        var activeListings = await _dbContext.Opportunities
            .CountAsync(o => o.BusinessProfileId == businessId && o.Status == OpportunityStatus.Published, cancellationToken);

        var totalListings = await _dbContext.Opportunities
            .CountAsync(o => o.BusinessProfileId == businessId, cancellationToken);

        var totalApplicants = await _dbContext.JobApplications
            .CountAsync(a => a.Opportunity.BusinessProfileId == businessId, cancellationToken);

        var pendingReviews = await _dbContext.JobApplications
            .CountAsync(a => a.Opportunity.BusinessProfileId == businessId && a.Opportunity.Status == OpportunityStatus.PendingApproval, cancellationToken);

        var shortlisted = await _dbContext.JobApplications
            .CountAsync(a => a.Opportunity.BusinessProfileId == businessId && a.Opportunity.Status == OpportunityStatus.Published, cancellationToken);

        var topListings = await _dbContext.Opportunities
            .Where(o => o.BusinessProfileId == businessId)
            .OrderByDescending(o => o.Applications.Count)
            .Take(5)
            .Select(o => new JobListingPerformanceDto(
                o.Id,
                o.Title,
                o.Applications.Count,
                o.CreatedAtUtc,
                o.Status.ToString()
            ))
            .ToListAsync(cancellationToken);

        return new BusinessAnalyticsDto(
            BusinessId: businessId,
            ActiveJobListings: activeListings,
            TotalJobPostings: totalListings,
            TotalApplicantsReceived: totalApplicants,
            PendingApplicantReviews: pendingReviews,
            ShortlistedCandidatesCount: shortlisted,
            ProfileViewsCount: 0,
            TopListings: topListings
        );
    }
}