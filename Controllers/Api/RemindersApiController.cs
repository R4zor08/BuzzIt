using BuzzIt.Extensions;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers.Api;

[ApiController]
[Authorize]
[Route("api/reminders")]
public class RemindersApiController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public RemindersApiController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    private int? CurrentUserId => User.GetUserId();

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        var reminders = await _reminderService.GetAllRemindersAsync(userId.Value);
        return Ok(reminders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        var reminder = await _reminderService.GetReminderByIdAsync(userId.Value, id);
        if (reminder == null)
        {
            return NotFound();
        }

        return Ok(reminder);
    }

    [HttpPatch("{id:int}/mark-done")]
    public async Task<IActionResult> MarkAsDone(int id)
    {
        var userId = CurrentUserId;
        if (userId is null)
        {
            return Unauthorized();
        }

        var reminder = await _reminderService.GetReminderByIdAsync(userId.Value, id);
        if (reminder == null)
        {
            return NotFound();
        }

        if (!reminder.IsCompleted)
        {
            await _reminderService.MarkAsDoneAsync(userId.Value, id);
        }

        return NoContent();
    }
}
