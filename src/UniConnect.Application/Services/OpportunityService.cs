using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class OpportunityService : IOpportunityService
{
    private readonly IUnitOfWork _unitOfWork;

    public OpportunityService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<IEnumerable<Opportunity>> GetPendingOpportunitiesAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Opportunities.GetPendingOpportunitiesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Opportunity>> GetMyOpportunitiesAsync(Guid businessProfileId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Opportunities.GetByBusinessProfileIdAsync(businessProfileId, cancellationToken);
    }

    public async Task CreateOpportunityAsync(Guid businessProfileId, CreateOpportunityDto dto, CancellationToken cancellationToken = default)
    {
        var opportunity = new Opportunity
        {
            Id = Guid.NewGuid(),
            BusinessProfileId = businessProfileId,
            Title = dto.Title,
            Description = dto.Description,
            TargetProgramme = dto.TargetProgramme,
            Status = OpportunityStatus.PendingApproval,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Opportunities.AddAsync(opportunity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApplyForJobAsync(Guid opportunityId, Guid applicantId, string cvFileUrl, CancellationToken cancellationToken = default)
    {
        var opportunity = await _unitOfWork.Opportunities.GetByIdAsync(opportunityId, cancellationToken);
        if (opportunity == null || opportunity.Status != OpportunityStatus.Published)
        {
            throw new KeyNotFoundException("Opportunity is not active or does not exist.");
        }
        if (!StorageValidationUtility.IsValidCvFileKey(cvFileUrl))
            throw new InvalidOperationException("Invalid cv format, save a cv to profile before you apply");

        var hasApplied = await _unitOfWork.Opportunities.HasUserAppliedAsync(opportunityId, applicantId, cancellationToken);
        if (hasApplied)
        {
            throw new InvalidOperationException("User has already applied for this role.");
        }

        var application = new JobApplication
        {
            Id = Guid.NewGuid(),
            OpportunityId = opportunityId,
            ApplicantId = applicantId,
            CvFileUrl = cvFileUrl,
            AppliedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Opportunities.AddApplicationAsync(application, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<Opportunity>> GetActiveOpportunitiesAsync(string? targetProgramme, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Opportunities.GetActiveOpportunitiesAsync(targetProgramme, cancellationToken);
    }

    public async Task<Opportunity?> GetByIdAsync(Guid opportunityId, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Opportunities.GetByIdAsync(opportunityId, cancellationToken);
    }

    public async Task ApproveOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken = default)
    {
        var opportunity = await GetRequiredOpportunityAsync(opportunityId, cancellationToken);
        opportunity.Status = OpportunityStatus.Published;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken = default)
    {
        var opportunity = await GetRequiredOpportunityAsync(opportunityId, cancellationToken);
        opportunity.Status = OpportunityStatus.Rejected;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task CloseOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken = default)
    {
        var opportunity = await GetRequiredOpportunityAsync(opportunityId, cancellationToken);
        opportunity.Status = OpportunityStatus.Closed;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Opportunity> GetRequiredOpportunityAsync(Guid opportunityId, CancellationToken cancellationToken)
    {
        var opportunity = await _unitOfWork.Opportunities.GetByIdAsync(opportunityId, cancellationToken);
        return opportunity ?? throw new KeyNotFoundException($"Opportunity with ID {opportunityId} was not found.");
    }

    public async Task<GetOpportunityWithApplications> GetOppporttunityWithApplication(Guid opportunityId, CancellationToken token)
    {
        var opportunityWithApps = await _unitOfWork.Opportunities.GetWithApplicationsAsync(opportunityId, token);

        if (opportunityWithApps == null)
            throw new InvalidOperationException("Opportunity was not found");

        return new GetOpportunityWithApplications(
            opportunityWithApps.Id,
            opportunityWithApps.BusinessProfile.UserId,
            opportunityWithApps.BusinessProfileId,
            opportunityWithApps.Title,
            opportunityWithApps.Description,
            opportunityWithApps.Applications?.Select(app => new ApplicantDto(
                app.Applicant?.Profile?.Id ?? Guid.Empty,
                app.Applicant?.Profile?.FirstName ?? string.Empty,
                app.Applicant?.Profile?.LastName ?? string.Empty,
                app.Applicant?.Profile?.SystemHeadline ?? string.Empty,
                app.Applicant?.Profile?.AboutBio ?? string.Empty,
                app.Applicant?.Profile?.CvFileUrl ?? string.Empty
            )) ?? Enumerable.Empty<ApplicantDto>()
        );
    }
}