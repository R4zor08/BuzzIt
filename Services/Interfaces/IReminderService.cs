using System.Collections.Generic;
using System.Threading.Tasks;
using BuzzIt.DTOs;
using BuzzIt.Models;

namespace BuzzIt.Services.Interfaces
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderDto>> GetAllRemindersAsync();
        Task<IEnumerable<ReminderDto>> SearchRemindersAsync(string? searchTerm, Category? category, Priority? priority, bool? isCompleted);
        Task<IEnumerable<ReminderDto>> GetUpcomingRemindersAsync(int days = 7);
        Task<IEnumerable<ReminderDto>> GetOverdueRemindersAsync();
        Task<ReminderDto?> GetReminderByIdAsync(int id);
        Task AddReminderAsync(ReminderDto reminderDto);
        Task UpdateReminderAsync(ReminderDto reminderDto);
        Task DeleteReminderAsync(int id);
        Task MarkAsDoneAsync(int id);
    }
}