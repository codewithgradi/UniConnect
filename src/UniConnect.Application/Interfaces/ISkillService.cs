using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface ISkillService
{
    Task<IEnumerable<SkillDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<SkillDto> CreateAsync(CreateSkillDto dto, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}