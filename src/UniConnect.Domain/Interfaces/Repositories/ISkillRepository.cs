using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface ISkillRepository
{
    Task<Skill?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Skill?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<Skill>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Skill skill, CancellationToken cancellationToken = default);
    void Remove(Skill skill);
}