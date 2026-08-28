using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Application.DTOs;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class AnalyticsRepository : IAnalyticsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AnalyticsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AdminAnalyticsDto> GetAdminAnalyticsAsync(CancellationToken cancellationToken = default)
    {
        // 1. User metrics
        var totalUsers = await _dbContext.Users.CountAsync(cancellationToken);

        var totalStudents = await _dbContext.Users.Where(x => x.UserType == UserType.Student).CountAsync(cancellationToken);
        var totalBusinesses = await _dbContext.Users.Where(x => x.UserType == UserType.Business).CountAsync(cancellationToken);
        var totalAdmins = totalUsers - (totalStudents + totalBusinesses);

        var totalOpportunities = await _dbContext.Opportunities.CountAsync(cancellationToken);
        var publishedOpportunities = await _dbContext.Opportunities.CountAsync(o => o.Status == OpportunityStatus.Published, cancellationToken);
        var pendingOpportunities = await _dbContext.Opportunities.CountAsync(o => o.Status == OpportunityStatus.PendingApproval, cancellationToken);
        var closedOpportunities = await _dbContext.Opportunities.CountAsync(o => o.Status == OpportunityStatus.Closed, cancellationToken);
        var totalApplications = await _dbContext.JobApplications.CountAsync(cancellationToken);

        var totalPosts = await _dbContext.Posts.CountAsync(cancellationToken);
        var totalComments = await _dbContext.Posts.Select(c => c.Comments).CountAsync(cancellationToken);
        var totalLikes = await _dbContext.Posts.Select(c => c.Reactions).CountAsync(cancellationToken);
        var totalConnections = await _dbContext.Connections.CountAsync(c => c.Status == ConnectionStatus.Accepted, cancellationToken);
        var totalMessages = await _dbContext.DirectMessages.CountAsync(cancellationToken);

        // 4. Verification & Moderation queue
        var pendingVerifications = await _dbContext.Opportunities.Where(x => x.Status == OpportunityStatus.PendingApproval).CountAsync(cancellationToken);

        // 5. 7-Day User Growth trend
        var last7Days = DateTime.UtcNow.Date.AddDays(-6);

        var recentRegistrationsData = await _dbContext.Users
            .Where(u => u.CreatedAtUtc >= last7Days)
            .GroupBy(u => u.CreatedAtUtc.Date)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(r => r.Date)
            .ToListAsync(cancellationToken);

        var recentRegistrations = recentRegistrationsData
            .Select(r => new DailyRegistrationDto(r.Date, r.Count));

        return new AdminAnalyticsDto(
            TotalUsers: totalUsers,
            TotalStudents: totalStudents,
            TotalBusinesses: totalBusinesses,
            TotalAdmins: totalAdmins < 0 ? 0 : totalAdmins,
            TotalOpportunities: totalOpportunities,
            PublishedOpportunities: publishedOpportunities,
            PendingOpportunities: pendingOpportunities,
            ClosedOpportunities: closedOpportunities,
            TotalJobApplications: totalApplications,
            TotalPosts: totalPosts,
            TotalComments: totalComments,
            TotalLikes: totalLikes,
            TotalConnections: totalConnections,
            TotalDirectMessages: totalMessages,
            PendingBusinessVerifications: pendingVerifications,
            RecentRegistrations: recentRegistrations
        );
    }
}