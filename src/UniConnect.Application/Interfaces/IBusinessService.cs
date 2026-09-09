using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;

namespace UniConnect.Application.Services;

public interface IBusinessService
{
    Task UpdateBusinessProfile(Guid userId, BusinessProfileUpdateDto updated);
    Task<BusinessProfileDto?> GetByUserIdAsync(Guid userId);
    Task CreateBusinessProfileAsync(Guid userId, CreateBusinessDto dto);
}