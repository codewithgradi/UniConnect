using System.Text.Json.Serialization;
using UniConnect.Domain.Enums;
namespace UniConnect.Application.DTOs;
public record PostOpportunitydto
(
    string Title,string Description, string TargetProgramme
);
public record LoginRequestDto(
    string Email,
    string Password
);
public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string UserType,
    string VerificationStatus
);

public record CreateProfileDto(
    string FirstName,
    string LastName,
    string StudentNumber,
    string Programme,
    string Headline,
    string Bio
);

public record UpdateProfileDto(
    string FirstName,
    string LastName,
    string StudentNumber,
    string Headline,
    string Bio,
    string Programme
);

public record AddExperienceDto(
    string Title,
    string CompanyName,
    string Location,
    DateTime StartDate,
    DateTime? EndDate,
    bool IsCurrent);

public record ExperienceDto(
    Guid Id,
    string Title,
    string Company,
    DateTime StartDate,
    DateTime? EndDate,
    bool IsCurrent
);

public record AddCertificationDto(
    string Name,
    string IssuingOrganization,
    DateTime IssueDate,
    string CredentialUrl
);

public record CertificationDto(
    Guid Id,
    string Name,
    string IssuingOrganization,
    DateTime IssueDate,
    string CredentialUrl
);

public record DetailedUserProfileDto(
    Guid Id,
    string UserId,
    string FirstName,
    string LastName,
    string StudentNumber,
    string Programme,
    string SystemHeadline,
    string AboutBio,
    string CvFileUrl,

IEnumerable<ExperienceDto> Experiences,
    IEnumerable<CertificationDto> Certifications,
    IEnumerable<SkillDto> Skills
);
public record SearchQuery(string searchItem, string targetProgramme);
public record UserProfileDto(Guid Id, string UserId, string FirstName, string LastName, string Headline, string Bio, string Programme, string StudentNumber);
public record BusinessProfileUpdateDto(string RegistrationNumber, string CompanyName, string Industry, string WebsiteUrl);

public record BusinessProfileDto(Guid Id, string CompanyName, string Industry, string WebsiteUrl);
public record CreateBusinessDto(string CompanyName, string RegistrationNumber, string Industry, string WebsiteUrl);

public record PostDto(
    Guid Id, 
    Guid AuthorId, 
    string Content, 
    DateTime CreatedAt,
    int CommentCount,
    int LikeCount,
    string FirstName,
    string LastName,
    string userEmail,
    bool isLiked
    );
public record ConnectionDto(Guid Id, Guid RequesterId, string FirtName, string LastName, ConnectionStatus Status);

public record ApplicationUserDto( UserProfileDto Profile);
public record UserProfiledto(string FirstName, string LastName);
public record DirectMessageDto(Guid Id, Guid SenderId, Guid ReceiverId, string Content, DateTime SentAt, bool IsRead);

public record CreateOpportunityDto(string Title, string Description, string TargetProgramme);

public record CreateEventDto(string Title, string Description, DateTime EventDate);
public record EventDto(Guid Id, string Title, string Description, DateTime EventDate);
public record SkillDto(Guid Id, string Name);
public record EmailRequest
{
    public required string To { get; init; }
    public required string Subject { get; init; }
    public required string HtmlBody { get; init; }
    public string? TextBody { get; init; }
    public string? From { get; init; }
}
public record CreateSkillDto(string Name);

public record SendMessageDto(
    Guid ReceiverId,
    string Content
);
public record MarkAsReadDto(
    IEnumerable<Guid> MessageIds
);
public record ApplyJobDto(
    string CvFileUrl
);
public record RegisterRequestDto(
    string Email,
    string Password,
    [property: JsonConverter(typeof(JsonStringEnumConverter))] UserType UserType,
    string? FirstName=null,
    string? LastName=null,
    string? Programme=null,
    string? CompanyName = null,
    string? studentNumber = null
);
public record GetOpportunityWithApplications(
    Guid Id,
    Guid UserId,
    Guid BusinessProfileId,
    string Title,
    string Descriptiom,
    IEnumerable<ApplicantDto> Applicants
);
public record ApplicantDto(
    Guid UserProfileId,
    string FirstName, 
    string LastName, 
    string SystemHeadline, 
    string AboutBio, 
    string CvFileUrl);

// DTOs/CertificateVerificationResponse.cs
public record CertificateVerificationResponse(
    int CredibilityScore,
    string DocumentStatus,
    DateTime Time
);

