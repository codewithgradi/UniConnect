namespace UniConnect.Application.DTOs;

public record UserProfileDto(Guid Id, string UserId, string FirstName, string LastName, string Headline, string Bio, string Programme, string StudentNumber);
public record UpdateProfileDto(string FirstName, string LastName, string Headline, string Bio, string StudentNumber);
public record CreateProfileDto(
    string FirstName,
    string LastName,
    string Programme,
    string? Headline,
    string? Bio,
    string? StudentNumber
);
public record BusinessProfileDto(Guid Id, string CompanyName, string Industry, string WebsiteUrl);
public record CreateBusinessDto(string CompanyName, string RegistrationNumber, string Industry, string WebsiteUrl);

public record PostDto(Guid Id, Guid AuthorId, string Content, DateTime CreatedAt, int CommentCount, int LikeCount);

public record DirectMessageDto(Guid Id, Guid SenderId, Guid ReceiverId, string Content, DateTime SentAt, bool IsRead);

public record CreateOpportunityDto(string Title, string Description, string TargetProgramme);

public record CreateEventDto(string Title, string Description, DateTime EventDate);
public record EventDto(Guid Id, string Title, string Description, DateTime EventDate);