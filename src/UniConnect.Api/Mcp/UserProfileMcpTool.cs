using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Server;
using UniConnect.Application.DTOs;
using UniConnect.Application.Services;

namespace UniConnect.Api.Mcp;

[McpServerToolType]
public class UserProfileMcpTool
{
    private readonly IServiceProvider _serviceProvider;

    public UserProfileMcpTool(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    [McpServerTool(Name = "get_user_full_profile")]
    [Description("Retrieves the complete profile of a student or alumnus, including basic info, bio, education, work experiences, certifications, and endorsed skills.")]
    public async Task<DetailedUserProfileDto?> GetFullUserProfile(
        [Description("The unique identifier (Guid) of the student or alumnus.")] Guid userId,
        CancellationToken cancellationToken = default)
    {
        return await GetUserProfileAsyncGeneral(userId, cancellationToken);
    }
    [McpServerTool(Name ="get_user_cv_url"),
    Description("This tool returns user cv url saved in userprofile table")
    ]
    public async Task<string> GetUserCvUrl(Guid userId, CancellationToken cancellationToken)
    {
        var profile = await GetUserProfileAsyncGeneral(userId, cancellationToken);
        return profile.CvFileUrl;
    }
    private async Task<DetailedUserProfileDto> GetUserProfileAsyncGeneral(Guid userId, CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var profileService = scope.ServiceProvider.GetRequiredService<IProfileService>();

        return await profileService.GetProfileByUserIdAsync(userId, cancellationToken);
    }
}