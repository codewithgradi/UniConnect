using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniConnect.Application.DTOs;
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
    public async Task<IActionResult> CreatePost([FromBody] CreatePostRequest request)
    {
        await _postService.CreatePostAsync(CurrentUserId, request.Content);
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

public record CreatePostRequest(string Content);
public record AddCommentRequest(string Content);
public record ToggleReactionRequest(string ReactionType);