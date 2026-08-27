using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class InstitutionalService : IInstitutionalService
{
    private readonly IUnitOfWork _unitOfWork;

    public InstitutionalService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateEventAsync(Guid adminId, CreateEventDto dto)
    {
        var campusEvent = new InstitutionalEvent
        {
            Id = Guid.NewGuid(),
            CreatedByAdminId = adminId,
            Title = dto.Title,
            Description = dto.Description,
            EventDate = dto.EventDate,
            IsPublished = true
        };

        await _unitOfWork.InstitutionalEvents.AddEventAsync(campusEvent);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<EventDto>> GetUpcomingEventsAsync()
    {
        var events = await _unitOfWork.InstitutionalEvents.GetUpcomingEventsAsync();
        return events.Select(e => new EventDto(e.Id, e.Title, e.Description, e.EventDate));
    }
}