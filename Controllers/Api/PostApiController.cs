using System.ComponentModel.DataAnnotations;
using BuzzIt.Models;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
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

    [HttpPost]
    public IActionResult Create([FromBody] PostApiRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var post = request.ToPost();
        _postService.Create(post);

        return CreatedAtAction(nameof(GetById), new { id = post.Id }, post);
    }

    [HttpPut("{id:int}")]
    public IActionResult Update(int id, [FromBody] PostApiRequest request)
    {
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
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

        _postService.Update(existing);
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        var existing = _postService.GetById(id);
        if (existing == null)
        {
            return NotFound();
        }

        _postService.Delete(id);
        return NoContent();
    }

    public class PostApiRequest
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        [Range(0, 4)]
        public Category Category { get; set; } = Category.General;

        [Range(0, 2)]
        public Priority Priority { get; set; } = Priority.Medium;

        public DateTime? DueDate { get; set; }

        public Post ToPost()
        {
            return new Post
            {
                Title = Title,
                Content = Content,
                Category = Category,
                Priority = Priority,
                DueDate = DueDate
            };
        }
    }
}
