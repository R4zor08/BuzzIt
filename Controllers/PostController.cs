using BuzzIt.Extensions;
using BuzzIt.Models;
using BuzzIt.Requests;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers;

[Authorize]
public class PostController : Controller
{
    private readonly IPostService _postService;

    public PostController(IPostService postService)
    {
        _postService = postService;
    }

    private IActionResult? RequireUserId(out int userId)
    {
        var id = User.GetUserId();
        if (id is null)
        {
            userId = 0;
            return Challenge();
        }

        userId = id.Value;
        return null;
    }

    [HttpGet]
    public IActionResult Index([FromQuery] SearchPostRequest request)
    {
        var auth = RequireUserId(out var userId);
        if (auth != null)
        {
            return auth;
        }

        if (!request.IsValid())
        {
            foreach (var error in request.GetErrors())
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        var posts = _postService.Search(userId, request.SearchTerm, request.Category, request.Priority, request.IsCompleted);

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
        var auth = RequireUserId(out var userId);
        if (auth != null)
        {
            return auth;
        }

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
            _postService.Create(userId, post);
            return RedirectToAction(nameof(Index));
        }
        return View(request);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var auth = RequireUserId(out var userId);
        if (auth != null)
        {
            return auth;
        }

        var post = _postService.GetById(userId, id);
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
        var auth = RequireUserId(out var userId);
        if (auth != null)
        {
            return auth;
        }

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
            var existing = _postService.GetById(userId, request.Id);
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

            var post = request.ToPost();
            try
            {
                _postService.Update(userId, post);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
        return View(request);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        var auth = RequireUserId(out var userId);
        if (auth != null)
        {
            return auth;
        }

        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Invalid post.";
            return RedirectToAction(nameof(Index));
        }

        var existing = _postService.GetById(userId, id);
        if (existing == null)
        {
            TempData["ErrorMessage"] = "Post not found.";
            return RedirectToAction(nameof(Index));
        }

        _postService.Delete(userId, id);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult MarkAsDone(int id)
    {
        var auth = RequireUserId(out var userId);
        if (auth != null)
        {
            return auth;
        }

        if (id <= 0)
        {
            TempData["ErrorMessage"] = "Invalid post.";
            return RedirectToAction(nameof(Index));
        }

        var existing = _postService.GetById(userId, id);
        if (existing == null)
        {
            TempData["ErrorMessage"] = "Post not found.";
            return RedirectToAction(nameof(Index));
        }

        _postService.MarkAsDone(userId, id);
        return RedirectToAction(nameof(Index));
    }
}
