using BuzzIt.DTOs;
using BuzzIt.Extensions;
using BuzzIt.Requests;
using BuzzIt.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BuzzIt.Controllers
{
    [Authorize]
    public class ReminderController : Controller
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
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
        public async Task<IActionResult> Index([FromQuery] SearchReminderRequest request)
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

            var reminders = await _reminderService.SearchRemindersAsync(
                userId,
                request.SearchTerm,
                request.Category,
                request.Priority,
                request.IsCompleted);

            ViewBag.SearchTerm = request.SearchTerm;
            ViewBag.Category = request.Category;
            ViewBag.Priority = request.Priority;
            ViewBag.IsCompleted = request.IsCompleted;

            return View(reminders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var auth = RequireUserId(out var userId);
            if (auth != null)
            {
                return auth;
            }

            var reminder = await _reminderService.GetReminderByIdAsync(userId, id);
            if (reminder == null)
            {
                return NotFound();
            }
            return View(reminder);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateReminderRequest());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateReminderRequest request)
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
                var reminderDto = new ReminderDto
                {
                    Title = request.Title,
                    Time = request.Time,
                    Description = request.Description,
                    Category = request.Category,
                    Priority = request.Priority,
                    DueDate = request.DueDate
                };
                await _reminderService.AddReminderAsync(userId, reminderDto);
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var auth = RequireUserId(out var userId);
            if (auth != null)
            {
                return auth;
            }

            var reminder = await _reminderService.GetReminderByIdAsync(userId, id);
            if (reminder == null)
            {
                return NotFound();
            }

            var updateRequest = new UpdateReminderRequest
            {
                Id = reminder.Id,
                Title = reminder.Title,
                Time = reminder.Time,
                Description = reminder.Description,
                Category = reminder.Category,
                Priority = reminder.Priority,
                DueDate = reminder.DueDate,
                IsCompleted = reminder.IsCompleted,
                CompletedAt = reminder.CompletedAt
            };

            return View(updateRequest);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UpdateReminderRequest request)
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
                try
                {
                    var reminderDto = new ReminderDto
                    {
                        Id = request.Id,
                        Title = request.Title,
                        Time = request.Time,
                        Description = request.Description,
                        Category = request.Category,
                        Priority = request.Priority,
                        DueDate = request.DueDate,
                        IsCompleted = request.IsCompleted,
                        CompletedAt = request.CompletedAt
                    };
                    await _reminderService.UpdateReminderAsync(userId, reminderDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }
            }
            return View(request);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var auth = RequireUserId(out var userId);
            if (auth != null)
            {
                return auth;
            }

            var reminder = await _reminderService.GetReminderByIdAsync(userId, id);
            if (reminder == null)
            {
                return NotFound();
            }
            return View(reminder);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] DeleteReminderRequest request)
        {
            var auth = RequireUserId(out var userId);
            if (auth != null)
            {
                return auth;
            }

            if (!request.IsValid())
            {
                return BadRequest(request.GetErrors());
            }

            try
            {
                await _reminderService.DeleteReminderAsync(userId, request.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDone(int id)
        {
            var auth = RequireUserId(out var userId);
            if (auth != null)
            {
                return auth;
            }

            if (id <= 0)
            {
                return BadRequest();
            }

            try
            {
                await _reminderService.MarkAsDoneAsync(userId, id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}
