using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IBusinessProfileRepository
{
    Task<BusinessProfile?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessProfile?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<BusinessProfile>> GetByIndustryAsync(string industry, CancellationToken cancellationToken = default);
    Task AddAsync(BusinessProfile businessProfile, CancellationToken cancellationToken = default);
    void Update(BusinessProfile businessProfile);
}