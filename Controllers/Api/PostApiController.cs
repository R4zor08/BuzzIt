using BuzzIt.Extensions;
using BuzzIt.Models;
using BuzzIt.Requests;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/post")]
public class PostApiController : ControllerBase
{
    private readonly IPostService _postService;

    public PostApiController(IPostService postService)
    {
        _postService = postService;
    }

    private int? CurrentUserId => User.GetUserId();

    [HttpGet]
    public IActionResult GetAll()
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        return Ok(_postService.GetAll(userId.Value));
    }

    [HttpGet("{id:int}")]
    public IActionResult GetById(int id)
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        var post = _postService.GetById(userId.Value, id);
        return post == null ? NotFound() : Ok(post);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreatePostRequest? request)
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        if (request is null)
        {
            return BadRequest(new { error = "Request body is required." });
        }

        if (!Enum.IsDefined(typeof(Category), request.Category))
        {
            ModelState.AddModelError(nameof(CreatePostRequest.Category),
                "Category must be 0 (General), 1 (Work), 2 (Personal), 3 (Ideas), or 4 (Urgent).");
        }

        if (!Enum.IsDefined(typeof(Priority), request.Priority))
        {
            ModelState.AddModelError(nameof(CreatePostRequest.Priority),
                "Priority must be 0 (Low), 1 (Medium), or 2 (High).");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!request.IsValid())
        {
            foreach (var err in request.GetErrors())
            {
                ModelState.AddModelError(err.Key, err.Value);
            }

            return ValidationProblem(ModelState);
        }

        var post = request.ToPost();
        _postService.Create(userId.Value, post);
        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] UpdatePostRequest? request)
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        if (request is null)
        {
            return BadRequest(new { error = "Request body is required." });
        }

        request.Id = id;

        if (!Enum.IsDefined(typeof(Category), request.Category))
        {
            ModelState.AddModelError(nameof(UpdatePostRequest.Category),
                "Category must be 0 (General), 1 (Work), 2 (Personal), 3 (Ideas), or 4 (Urgent).");
        }

        if (!Enum.IsDefined(typeof(Priority), request.Priority))
        {
            ModelState.AddModelError(nameof(UpdatePostRequest.Priority),
                "Priority must be 0 (Low), 1 (Medium), or 2 (High).");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        if (!request.IsValid())
        {
            foreach (var err in request.GetErrors())
            {
                ModelState.AddModelError(err.Key, err.Value);
            }

            return ValidationProblem(ModelState);
        }

        var existing = _postService.GetById(userId.Value, id);
        if (existing == null)
        {
            return NotFound();
        }

        if (request.IsCompleted)
        {
            request.CompletedAt = existing.IsCompleted && existing.CompletedAt.HasValue
                ? existing.CompletedAt
                : DateTime.Now;
        }
        else
        {
            request.CompletedAt = null;
        }

        try
        {
            _postService.Update(userId.Value, request.ToPost());
        }
        catch (InvalidOperationException)
        {
            return NotFound();
        }

        var updated = _postService.GetById(userId.Value, id);
        return Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        var existing = _postService.GetById(userId.Value, id);
        if (existing == null)
        {
            return NotFound();
        }

        _postService.Delete(userId.Value, id);
        return NoContent();
    }

    [HttpPatch("{id:int}/mark-done")]
    public IActionResult MarkAsDone(int id)
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        var existing = _postService.GetById(userId.Value, id);
        if (existing == null)
        {
            return NotFound();
        }

        if (!existing.IsCompleted)
        {
            _postService.MarkAsDone(userId.Value, id);
        }

        return NoContent();
    }
}
