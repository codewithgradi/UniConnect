using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IBusinessService
{
    Task<BusinessProfileDto?> GetByUserIdAsync(Guid userId);
    Task CreateBusinessProfileAsync(Guid userId, CreateBusinessDto dto);
}