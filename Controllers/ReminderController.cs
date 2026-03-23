using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BuzzIt.DTOs;
using BuzzIt.Services.Interfaces;
using BuzzIt.Requests;

namespace BuzzIt.Controllers
{
    public class ReminderController : Controller
    {
        private readonly IReminderService _reminderService;

        public ReminderController(IReminderService reminderService)
        {
            _reminderService = reminderService;
        }

        // GET: Reminder
        [HttpGet]
        public async Task<IActionResult> Index([FromQuery] SearchReminderRequest request)
        {
            if (!request.IsValid())
            {
                foreach (var error in request.GetErrors())
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }
            }

            var reminders = await _reminderService.SearchRemindersAsync(
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

        // GET: Reminder/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var reminder = await _reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }
            return View(reminder);
        }

        // GET: Reminder/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateReminderRequest());
        }

        // POST: Reminder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateReminderRequest request)
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
                var reminderDto = new ReminderDto
                {
                    Title = request.Title,
                    Time = request.Time,
                    Description = request.Description,
                    Category = request.Category,
                    Priority = request.Priority,
                    DueDate = request.DueDate
                };
                await _reminderService.AddReminderAsync(reminderDto);
                return RedirectToAction(nameof(Index));
            }
            return View(request);
        }

        // GET: Reminder/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var reminder = await _reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }

            var request = new UpdateReminderRequest
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

            return View(request);
        }

        // POST: Reminder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([FromForm] UpdateReminderRequest request)
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
                    await _reminderService.UpdateReminderAsync(reminderDto);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException)
                {
                    return NotFound();
                }
            }
            return View(request);
        }

        // GET: Reminder/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var reminder = await _reminderService.GetReminderByIdAsync(id);
            if (reminder == null)
            {
                return NotFound();
            }
            return View(reminder);
        }

        // POST: Reminder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed([FromForm] DeleteReminderRequest request)
        {
            if (!request.IsValid())
            {
                return BadRequest(request.GetErrors());
            }

            try
            {
                await _reminderService.DeleteReminderAsync(request.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }

        // POST: Reminder/MarkAsDone/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsDone([FromForm] MarkReminderDoneRequest request)
        {
            if (!request.IsValid())
            {
                return BadRequest(request.GetErrors());
            }

            try
            {
                await _reminderService.MarkAsDoneAsync(request.Id);
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }
}