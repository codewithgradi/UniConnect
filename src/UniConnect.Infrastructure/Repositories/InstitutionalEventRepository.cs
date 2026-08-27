using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class InstitutionalEventRepository : RepositoryBase<InstitutionalEvent>, IInstitutionalEventRepository
{
    public InstitutionalEventRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<InstitutionalEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.InstitutionalEvents
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<InstitutionalEvent>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.InstitutionalEvents
            .Where(e => e.IsPublished && e.EventDate >= DateTime.UtcNow)
            .OrderBy(e => e.EventDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Announcement>> GetRecentAnnouncementsAsync(int count = 10, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Announcements
            .OrderByDescending(a => a.BroadcastAtUtc)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task AddEventAsync(InstitutionalEvent campusEvent, CancellationToken cancellationToken = default)
    {
        await _dbContext.InstitutionalEvents.AddAsync(campusEvent, cancellationToken);
    }

    public async Task AddAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken = default)
    {
        await _dbContext.Announcements.AddAsync(announcement, cancellationToken);
    }
}