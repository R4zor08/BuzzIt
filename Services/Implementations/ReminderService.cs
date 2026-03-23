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

        public async Task<IEnumerable<ReminderDto>> GetAllRemindersAsync()
        {
            return await _context.Reminders
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.DueDate)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<IEnumerable<ReminderDto>> SearchRemindersAsync(string? searchTerm, Category? category, Priority? priority, bool? isCompleted)
        {
            var query = _context.Reminders.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.Trim().ToLowerInvariant();
                query = query.Where(r => r.Title.ToLowerInvariant().Contains(searchTerm) ||
                                          r.Description.ToLowerInvariant().Contains(searchTerm));
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

        public async Task<IEnumerable<ReminderDto>> GetUpcomingRemindersAsync(int days = 7)
        {
            var now = DateTime.Now;
            var endDate = now.AddDays(days);

            return await _context.Reminders
                .Where(r => !r.IsCompleted && r.Time >= now && r.Time <= endDate)
                .OrderBy(r => r.Time)
                .ThenByDescending(r => r.Priority)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task<IEnumerable<ReminderDto>> GetOverdueRemindersAsync()
        {
            var now = DateTime.Now;

            return await _context.Reminders
                .Where(r => !r.IsCompleted && 
                           ((r.DueDate.HasValue && r.DueDate.Value.Date < now.Date) ||
                            (!r.DueDate.HasValue && r.Time < now)))
                .OrderByDescending(r => r.Priority)
                .ThenBy(r => r.DueDate)
                .Select(r => MapToDto(r))
                .ToListAsync();
        }

        public async Task AddReminderAsync(ReminderDto reminderDto)
        {
            var reminder = new Reminder
            {
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

        public async Task UpdateReminderAsync(ReminderDto reminderDto)
        {
            var reminder = await _context.Reminders.FindAsync(reminderDto.Id);
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

        public async Task DeleteReminderAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
                throw new InvalidOperationException("Reminder not found.");

            _context.Reminders.Remove(reminder);
            await _context.SaveChangesAsync();
        }

        public async Task<ReminderDto?> GetReminderByIdAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
            if (reminder == null)
                return null;

            return MapToDto(reminder);
        }

        public async Task MarkAsDoneAsync(int id)
        {
            var reminder = await _context.Reminders.FindAsync(id);
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