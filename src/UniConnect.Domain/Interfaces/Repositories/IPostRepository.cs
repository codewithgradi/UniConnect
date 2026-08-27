using UniConnect.Domain.Entities;

namespace UniConnect.Domain.Interfaces.Repositories;

public interface IPostRepository
{
    Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetFeedPostsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<IEnumerable<Post>> GetPostsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default);
    Task AddAsync(Post post, CancellationToken cancellationToken = default);
    void Delete(Post post);

    // Comment & Reaction Aggregates
    Task AddCommentAsync(Comment comment, CancellationToken cancellationToken = default);
    Task ToggleReactionAsync(Guid postId, Guid userId, string reactionType, CancellationToken cancellationToken = default);
}