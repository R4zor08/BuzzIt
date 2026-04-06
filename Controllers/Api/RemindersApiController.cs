using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers.Api;

[ApiController]
[Route("api/reminders")]
public class RemindersApiController : ControllerBase
{
    private readonly IReminderService _reminderService;

    public RemindersApiController(IReminderService reminderService)
    {
        _reminderService = reminderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var reminders = await _reminderService.GetAllRemindersAsync();
        return Ok(reminders);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var reminder = await _reminderService.GetReminderByIdAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        return Ok(reminder);
    }

    [HttpPatch("{id:int}/mark-done")]
    public async Task<IActionResult> MarkAsDone(int id)
    {
        var reminder = await _reminderService.GetReminderByIdAsync(id);
        if (reminder == null)
        {
            return NotFound();
        }

        if (!reminder.IsCompleted)
        {
            await _reminderService.MarkAsDoneAsync(id);
        }

        return NoContent();
    }
}
