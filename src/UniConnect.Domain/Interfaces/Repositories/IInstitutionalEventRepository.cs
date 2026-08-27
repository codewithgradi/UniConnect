using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IInstitutionalEventRepository
{
    Task<InstitutionalEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<InstitutionalEvent>> GetUpcomingEventsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Announcement>> GetRecentAnnouncementsAsync(int count = 10, CancellationToken cancellationToken = default);
    Task AddEventAsync(InstitutionalEvent campusEvent, CancellationToken cancellationToken = default);
    Task AddAnnouncementAsync(Announcement announcement, CancellationToken cancellationToken = default);
}