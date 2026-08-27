using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class BusinessService : IBusinessService
{
    private readonly IUnitOfWork _unitOfWork;

    public BusinessService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<BusinessProfileDto?> GetByUserIdAsync(Guid userId)
    {
        var business = await _unitOfWork.BusinessProfiles.GetByUserIdAsync(userId);
        if (business == null) return null;

        return new BusinessProfileDto(business.Id, business.CompanyName, business.Industry, business.WebsiteUrl);
    }

    public async Task CreateBusinessProfileAsync(Guid userId, CreateBusinessDto dto)
    {
        var business = new BusinessProfile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            CompanyName = dto.CompanyName,
            CompanyRegistrationNumber = dto.RegistrationNumber,
            Industry = dto.Industry,
            WebsiteUrl = dto.WebsiteUrl
        };

        await _unitOfWork.BusinessProfiles.AddAsync(business);
        await _unitOfWork.SaveChangesAsync();
    }
}