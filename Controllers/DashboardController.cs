using BuzzIt.Extensions;
using BuzzIt.Models;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly IPostService _postService;
    private readonly IReminderService _reminderService;

    public DashboardController(IPostService postService, IReminderService reminderService)
    {
        _postService = postService;
        _reminderService = reminderService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();
        if (userId is null)
        {
            return Challenge();
        }

        var uid = userId.Value;
        var model = new DashboardViewModel
        {
            TotalPosts = _postService.GetTotalCount(uid),
            PendingPosts = _postService.GetPendingCount(uid),
            CompletedPosts = _postService.GetCompletedCount(uid),
            TotalReminders = await _reminderService.GetTotalCountAsync(uid),
            PendingReminders = await _reminderService.GetPendingCountAsync(uid),
            CompletedReminders = await _reminderService.GetCompletedCountAsync(uid)
        };

        return View(model);
    }
}
