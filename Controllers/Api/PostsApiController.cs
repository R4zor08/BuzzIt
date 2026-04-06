using BuzzIt.Models;
using BuzzIt.Requests;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers.Api;

[ApiController]
[Route("api/posts")]
public class PostsApiController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsApiController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        var posts = _postService.GetAll();
        return Ok(posts);
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var post = _postService.GetById(id);
        if (post == null)
        {
            return NotFound();
        }

        return Ok(post);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdatePostRequest request)
    {
        if (request == null)
        {
            return BadRequest(new { error = "Request body is required." });
        }

        request.Id = id;
        if (!request.IsValid())
        {
            return BadRequest(request.GetErrors());
        }

        var existing = _postService.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        existing.Title = request.Title;
        existing.Content = request.Content;
        existing.Category = request.Category;
        existing.Priority = request.Priority;
        existing.DueDate = request.DueDate;
        existing.IsCompleted = request.IsCompleted;
        existing.CompletedAt = request.CompletedAt;

        _postService.Update(existing);
        return Ok(existing);
    }

    [HttpPatch("{id:int}/mark-done")]
    public IActionResult MarkAsDone(int id)
    {
        var existing = _postService.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        if (!existing.IsCompleted)
        {
            _postService.MarkAsDone(id);
        }

        return NoContent();
    }
}
