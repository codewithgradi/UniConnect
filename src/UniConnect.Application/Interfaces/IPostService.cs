using UniConnect.Application.DTOs;

namespace UniConnect.Application.Services;

public interface IPostService
{
    Task CreatePostAsync(Guid authorId, string content);
    Task AddCommentAsync(Guid postId, Guid authorId, string content);
    Task ToggleReactionAsync(Guid postId, Guid userId, string reactionType);
    Task<IEnumerable<PostDto>> GetFeedAsync(int pageNumber, int pageSize);
}