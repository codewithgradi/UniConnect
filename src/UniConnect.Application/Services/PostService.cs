using UniConnect.Application.DTOs;
using UniConnect.Domain.Entities;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;

    public PostService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CreatePostAsync(Guid authorId, string content)
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            Content = content,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Posts.AddAsync(post);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task AddCommentAsync(Guid postId, Guid authorId, string content)
    {
        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            AuthorId = authorId,
            Content = content,
            CreatedAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Posts.AddCommentAsync(comment);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task ToggleReactionAsync(Guid postId, Guid userId, string reactionType)
    {
        await _unitOfWork.Posts.ToggleReactionAsync(postId, userId, reactionType);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<IEnumerable<PostDto>> GetFeedAsync(int pageNumber, int pageSize)
    {
        var posts = await _unitOfWork.Posts.GetFeedPostsAsync(pageNumber, pageSize);
        return posts.Select(p => new PostDto(p.Id, p.AuthorId, p.Content, p.CreatedAtUtc, p.Comments.Count, p.Reactions.Count));
    }
}