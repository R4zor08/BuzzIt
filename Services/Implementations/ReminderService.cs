using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BuzzIt.Data;
using BuzzIt.DTOs;
using BuzzIt.Models;
using BuzzIt.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuzzIt.Services.Implementations
{
    public class ReminderService : IReminderService
    {
        private readonly ApplicationDbContext _context;

        public ReminderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ReminderDto>> GetAllRemindersAsync(int userId)
        {
            return await BaseQuery(userId)
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.DueDate)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<IEnumerable<ReminderDto>> SearchRemindersAsync(int userId, string? searchTerm, Category? category, Priority? priority, bool? isCompleted)
        {
            var query = BaseQuery(userId);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.Trim().ToLowerInvariant();
                query = query.Where(r => r.Title.ToLower().Contains(term) ||
                                          r.Description.ToLower().Contains(term));
            }

            if (category.HasValue)
            {
                query = query.Where(r => r.Category == category.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(r => r.Priority == priority.Value);
            }

            if (isCompleted.HasValue)
            {
                query = query.Where(r => r.IsCompleted == isCompleted.Value);
            }

            return await query
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.DueDate)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<IEnumerable<ReminderDto>> GetUpcomingRemindersAsync(int userId, int days = 7)
        {
            var now = DateTime.Now;
            var endDate = now.AddDays(days);

            return await BaseQuery(userId)
                .Where(r => !r.IsCompleted && r.Time >= now && r.Time <= endDate)
                .OrderBy(r => r.Time)
                .ThenByDescending(r => r.Priority)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<IEnumerable<ReminderDto>> GetOverdueRemindersAsync(int userId)
        {
            var now = DateTime.Now;

            return await BaseQuery(userId)
                .Where(r => !r.IsCompleted &&
                           ((r.DueDate.HasValue && r.DueDate.Value.Date < now.Date) ||
                            (!r.DueDate.HasValue && r.Time < now)))
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.DueDate)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task AddReminderAsync(int userId, ReminderDto reminderDto)
        {
            var reminder = new Reminder
            {
                UserId = userId,
                Title = reminderDto.Title,
                Time = reminderDto.Time,
                Description = reminderDto.Description,
                IsCompleted = reminderDto.IsCompleted,
                Category = reminderDto.Category,
                Priority = reminderDto.Priority,
                DueDate = reminderDto.DueDate
            };

            await _context.Reminders.AddAsync(reminder);
            await _context.SaveChangesAsync();

            reminderDto.Id = reminder.Id;
        }

        public async Task UpdateReminderAsync(int userId, ReminderDto reminderDto)
        {
            var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.Id == reminderDto.Id && r.UserId == userId);
            if (reminder == null)
                throw new InvalidOperationException("Reminder not found.");

            reminder.Title = reminderDto.Title;
            reminder.Time = reminderDto.Time;
            reminder.Description = reminderDto.Description;
            reminder.IsCompleted = reminderDto.IsCompleted;
            reminder.CompletedAt = reminderDto.CompletedAt;
            reminder.Category = reminderDto.Category;
            reminder.Priority = reminderDto.Priority;
            reminder.DueDate = reminderDto.DueDate;

            _context.Reminders.Update(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteReminderAsync(int userId, int id)
        {
            var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (reminder == null)
                throw new InvalidOperationException("Reminder not found.");

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task<ReminderDto?> GetReminderByIdAsync(int userId, int id)
        {
            var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (reminder == null)
                return null;

            return MapToDto(reminder);
        }

        public async Task MarkAsDoneAsync(int userId, int id)
        {
            var reminder = await _context.Reminders.FirstOrDefaultAsync(r => r.Id == id && r.UserId == userId);
            if (reminder == null)
                throw new InvalidOperationException("Reminder not found.");

            if (!reminder.IsCompleted)
            {
                reminder.IsCompleted = true;
                reminder.CompletedAt = DateTime.Now;
                _context.Reminders.Update(reminder);
                await _context.SaveChangesAsync();
            }
        }

        public Task<int> GetTotalCountAsync(int userId) =>
            _context.Reminders.CountAsync(r => r.UserId == userId);

        public Task<int> GetPendingCountAsync(int userId) =>
            _context.Reminders.CountAsync(r => r.UserId == userId && !r.IsCompleted);

        public Task<int> GetCompletedCountAsync(int userId) =>
            _context.Reminders.CountAsync(r => r.UserId == userId && r.IsCompleted);

        private IQueryable<Reminder> BaseQuery(int userId) =>
            _context.Reminders.Where(r => r.UserId == userId);

        private static ReminderDto MapToDto(Reminder r) => new ReminderDto
        {
            Id = r.Id,
            Title = r.Title,
            Time = r.Time,
            Description = r.Description,
            IsCompleted = r.IsCompleted,
            CompletedAt = r.CompletedAt,
            Category = r.Category,
            Priority = r.Priority,
            DueDate = r.DueDate
        };
    }
}
