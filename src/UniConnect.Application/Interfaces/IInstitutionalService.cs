using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IInstitutionalService
{
    Task CreateEventAsync(Guid adminId, CreateEventDto dto);
    Task<IEnumerable<EventDto>> GetUpcomingEventsAsync();
}