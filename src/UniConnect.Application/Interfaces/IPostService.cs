using UniConnect.Application.DTOs;
using UniConnect.Application.Interfaces;

namespace UniConnect.Application.Services;

public interface IPostService
{
    Task CreatePostAsync(Guid authorId, string content, FileUploadDto? media, CancellationToken token);
    Task AddCommentAsync(Guid postId, Guid authorId, string content);
    Task ToggleReactionAsync(Guid postId, Guid userId, string reactionType);
    Task<IEnumerable<PostDto>> GetFeedAsync(Guid userId,int pageNumber, int pageSize);
}