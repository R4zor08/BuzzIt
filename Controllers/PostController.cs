using Microsoft.AspNetCore.Mvc;
using BuzzIt.Models;
using BuzzIt.Services.Interfaces;
using BuzzIt.Requests;

namespace BuzzIt.Controllers;

public class PostController : Controller
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public IActionResult Index([FromQuery] SearchPostRequest request)
    {
        if (!request.IsValid())
        {
            foreach (var error in request.GetErrors())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        var posts = _postService.Search(request.SearchTerm, request.Category, request.Priority, request.IsCompleted);
        
        ViewBag.SearchTerm = request.SearchTerm;
        ViewBag.Category = request.Category;
        ViewBag.Priority = request.Priority;
        ViewBag.IsCompleted = request.IsCompleted;
        
        return View(posts);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreatePostRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create([FromForm] CreatePostRequest request)
    {
        if (!request.IsValid())
        {
            foreach (var error in request.GetErrors())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            return View(request);
        }

        if (ModelState.IsValid)
        {
            var post = request.ToPost();
            _postService.Create(post);
            return RedirectToAction(nameof(Index));
        }
        return View(request);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var post = _postService.GetById(id);
        if (post == null)
        {
            return NotFound();
        }

        var request = new UpdatePostRequest
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            Category = post.Category,
            Priority = post.Priority,
            DueDate = post.DueDate,
            IsCompleted = post.IsCompleted,
            CompletedAt = post.CompletedAt
        };

        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit([FromForm] UpdatePostRequest request)
    {
        if (!request.IsValid())
        {
            foreach (var error in request.GetErrors())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
            return View(request);
        }

        if (ModelState.IsValid)
        {
            var post = request.ToPost();
            _postService.Update(post);
            return RedirectToAction(nameof(Index));
        }
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete([FromForm] DeletePostRequest request)
    {
        if (!request.IsValid())
        {
            return BadRequest(request.GetErrors());
        }

        _postService.Delete(request.Id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkAsDone([FromForm] MarkPostDoneRequest request)
    {
        if (!request.IsValid())
        {
            return BadRequest(request.GetErrors());
        }

        _postService.MarkAsDone(request.Id);
        return RedirectToAction(nameof(Index));
    }
}
