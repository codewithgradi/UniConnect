using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class OpportunityService : IOpportunityService
{
    private readonly IUnitOfWork _unitOfWork;

    public OpportunityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreateOpportunityAsync(Guid businessProfileId, CreateOpportunityDto dto)
    {
        var opportunity = new Opportunity
        {
            Id = Guid.NewGuid(),
            BusinessProfileId = businessProfileId,
            Title = dto.Title,
            Description = dto.Description,
            TargetProgramme = dto.TargetProgramme,
            Status = Domain.Enums.OpportunityStatus.PendingApproval, // Active
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Opportunities.AddAsync(opportunity);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ApplyForJobAsync(Guid opportunityId, Guid applicantId, string cvFileUrl)
    {
        var hasApplied = await _unitOfWork.Opportunities.HasUserAppliedAsync(opportunityId, applicantId);
        if (hasApplied) throw new InvalidOperationException("User has already applied for this role.");

        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            OpportunityId = opportunityId,
            ApplicantId = applicantId,
            CvFileUrl = cvFileUrl,
            AppliedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Opportunities.AddApplicationAsync(application);
        await _unitOfWork.SaveChangesAsync();
    }
}