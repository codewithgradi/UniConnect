using System.Text.Json.Serialization;
using UniConnect.Domain.Enums;
namespace UniConnect.Application.DTOs;


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
public record UserProfileDto(Guid Id, string UserId, string FirstName, string LastName, string Headline, string Bio, string Programme, string StudentNumber);

public record BusinessProfileDto(Guid Id, string CompanyName, string Industry, string WebsiteUrl);
public record CreateBusinessDto(string CompanyName, string RegistrationNumber, string Industry, string WebsiteUrl);

public record PostDto(Guid Id, Guid AuthorId, string Content, DateTime CreatedAt, int CommentCount, int LikeCount);

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
    string? FirstName,
    string? LastName,
    string? Programme,
    string? CompanyName,
    string? studentNumber
);