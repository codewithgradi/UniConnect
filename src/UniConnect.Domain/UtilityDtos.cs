public record AdminAnalyticsDto(
    // System Totals
    int TotalUsers,
    int TotalStudents,
    int TotalBusinesses,
    int TotalAdmins,

    // Opportunities & Applications
    int TotalOpportunities,
    int PublishedOpportunities,
    int PendingOpportunities,
    int ClosedOpportunities,
    int TotalJobApplications,

    // Social & Messaging
    int TotalPosts,
    int TotalComments,
    int TotalLikes,
    int TotalConnections,
    int TotalDirectMessages,

    // Verification & Moderation
    int PendingBusinessVerifications,

    // Growth & Activity Trends
    IEnumerable<DailyRegistrationDto> RecentRegistrations
);

public record DailyRegistrationDto(
    DateTime Date,
    int Count
);

// Analytics tailored for an individual Student user
public record StudentAnalyticsDto(
    Guid StudentId,
    int AppliedJobsCount,
    int BookmarkedJobsCount,
    int TotalConnections,
    int PendingConnectionRequests,
    int TotalEndorsementsReceived,
    int ProfileViewsCount,
    IEnumerable<RecentApplicationStatusDto> RecentApplications
);

public record RecentApplicationStatusDto(
    Guid ApplicationId,
    string JobTitle,
    string CompanyName,
    string Status,
    DateTime AppliedAtUtc
);

// Analytics tailored for a Business user
public record BusinessAnalyticsDto(
    Guid BusinessId,
    int ActiveJobListings,
    int TotalJobPostings,
    int TotalApplicantsReceived,
    int PendingApplicantReviews,
    int ShortlistedCandidatesCount,
    int ProfileViewsCount,
    IEnumerable<JobListingPerformanceDto> TopListings
);

public record JobListingPerformanceDto(
    Guid OpportunityId,
    string Title,
    int ApplicantCount,
    DateTime PostedAtUtc,
    string Status
);