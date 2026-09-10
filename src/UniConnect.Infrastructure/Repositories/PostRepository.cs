using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Infrastructure.Repositories;

public class PostRepository : RepositoryBase<Post>, IPostRepository
{
    public PostRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Posts
            .Include(p => p.Comments)
            .Include(p => p.Reactions)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<Post>> GetFeedPostsAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Posts
        .Include(p => p.Author)
        .ThenInclude(a => a.Profile)
        .Include(p => p.Comments)
        .Include(p => p.Reactions)
        .OrderByDescending(p => p.CreatedAtUtc)
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

        
    }

    public async Task<IEnumerable<Post>> GetPostsByAuthorIdAsync(Guid authorId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Posts
            .Where(p => p.AuthorId == authorId)
            .OrderByDescending(p => p.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Post post, CancellationToken cancellationToken = default)
    {
        await _dbContext.Posts.AddAsync(post, cancellationToken);
    }

    public void Delete(Post post)
    {
        _dbContext.Posts.Remove(post);
    }

    public async Task AddCommentAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Comments.AddAsync(comment, cancellationToken);
    }

    public async Task ToggleReactionAsync(Guid postId, Guid userId, string reactionType, CancellationToken cancellationToken = default)
    {
        var existing = await _dbContext.Reactions
            .FirstOrDefaultAsync(r => r.PostId == postId && r.UserId == userId, cancellationToken);

        if (existing != null)
        {
            if (existing.ReactionType == reactionType)
            {
                _dbContext.Reactions.Remove(existing);
            }
            else
            {
                existing.ReactionType = reactionType;
            }
        }
        else
        {
            var reaction = new Reaction
            {
                Id = Guid.NewGuid(),
                PostId = postId,
                UserId = userId,
                ReactionType = reactionType
            };
            await _dbContext.Reactions.AddAsync(reaction, cancellationToken);
        }
    }

    public async Task<int> GetPostCount(Guid userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Posts
            .Where(p => p.AuthorId == userId)
            .CountAsync(cancellationToken);
    }
}