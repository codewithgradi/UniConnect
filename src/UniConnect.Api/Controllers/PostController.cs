using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
using UniConnect.Application.Interfaces;
using UniConnect.Application.Services;

namespace UniConnect.Api.Controllers;

[Authorize]
[Tags("Posts & Feed")]
public class PostsController : ApiControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet("feed")]
    public async Task<IActionResult> GetFeed([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
    {

        var feed = await _postService.GetFeedAsync(CurrentUserId,pageNumber, pageSize);
        return Ok(feed);
    }
    [HttpPost]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> CreatePost(
        [FromForm] string? content,  // <-- Make this nullable
        [FromForm] IFormFile? mediaFile,
        CancellationToken token = default)
    {
        // Now this code will successfully catch empty/missing content
        if (string.IsNullOrWhiteSpace(content))
        {
            return BadRequest(new { Message = "Content parameter is missing or empty." });
        }

        FileUploadDto? mediaDto = null;
        if (mediaFile != null && mediaFile.Length > 0)
        {
            mediaDto = new FileUploadDto(
                mediaFile.OpenReadStream(),
                mediaFile.FileName,
                mediaFile.ContentType
            );
        }

        if (CurrentUserId == null) return Unauthorized();

        await _postService.CreatePostAsync(CurrentUserId, content, mediaDto, token);
        return Ok(new { Message = "Post created successfully." });
    }
    [HttpPost("{postId:guid}/comments")]
    public async Task<IActionResult> AddComment(Guid postId, [FromBody] AddCommentRequest request)
    {
        await _postService.AddCommentAsync(postId, CurrentUserId, request.Content);
        return Ok(new { Message = "Comment added." });
    }

    [HttpPost("{postId:guid}/react")]
    public async Task<IActionResult> ToggleReaction(Guid postId, [FromBody] ToggleReactionRequest request)
    {
        await _postService.ToggleReactionAsync(postId, CurrentUserId, request.ReactionType);
        return Ok(new { Message = "Reaction updated." });
    }
}

// Change this from a record to a standard class:
public class CreatePostRequest
{
    [Required(AllowEmptyStrings = true)] // Allows empty text when uploading media
    public string Content { get; set; } = string.Empty;

    public IFormFile? MediaFile { get; set; }
}
public record AddCommentRequest(string Content);
public record ToggleReactionRequest(string ReactionType);