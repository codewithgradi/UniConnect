using UniConnect.Application.DTOs;
using UniConnect.Application.Interfaces;
using UniConnect.Domain.Enums;
using UniConnect.Domain.Interfaces.Repositories;

namespace UniConnect.Application.Services;

public class PostService : IPostService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IR2StorageService _r2StorageService;

    public PostService(IUnitOfWork unitOfWork, IR2StorageService r2StorageService)
    {
        _unitOfWork = unitOfWork;
        _r2StorageService=r2StorageService;
    }

    public async Task CreatePostAsync(Guid authorId, string content, FileUploadDto? mediaDto, CancellationToken token =default)
    {
        string? mediaUrl = null;
        MediaType? mediaType = null;

        if (mediaDto != null && mediaDto.Content.Length > 0)
        {
            // Basic type validation
            if (mediaDto.ContentType.StartsWith("image/"))
            {
                mediaType = MediaType.Image;
            }
            else if (mediaDto.ContentType.StartsWith("video/"))
            {
                mediaType = MediaType.Video;
            }
            else
            {
                throw new ArgumentException("Invalid media type. Only images and videos are allowed.");
            }

            mediaUrl = await _r2StorageService.UploadMediaAsync(mediaDto, token);
        }
        var post = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = authorId,
            Content = content,
            MediaUrl = mediaUrl,
            MediaType= mediaType,
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

    public async Task<IEnumerable<PostDto>> GetFeedAsync(Guid userId,int pageNumber, int pageSize)
    {
        var posts = await _unitOfWork.Posts.GetFeedPostsAsync(pageNumber, pageSize);
        return posts.Select(p => new PostDto(
            p.Id,
            p.AuthorId,
            p.Content,
            p.CreatedAtUtc,
            p.Comments?.Count ?? 0,
            p.Reactions?.Count ?? 0,
            p.Author?.Profile?.FirstName ?? string.Empty,
            p.Author?.Profile?.LastName ?? string.Empty,
            p.Author?.Email ?? string.Empty,
            p.Reactions.Any(r => r.UserId == userId),
            p.MediaUrl,
            p.MediaType.ToString()

            ));
    }
}