using System.Collections.Generic;
using System.Threading.Tasks;
using BuzzIt.DTOs;
using BuzzIt.Models;

namespace BuzzIt.Services.Interfaces
{
    public interface IReminderService
    {
        Task<IEnumerable<ReminderDto>> GetAllRemindersAsync(int userId);
        Task<IEnumerable<ReminderDto>> SearchRemindersAsync(int userId, string? searchTerm, Category? category, Priority? priority, bool? isCompleted);
        Task<IEnumerable<ReminderDto>> GetUpcomingRemindersAsync(int userId, int days = 7);
        Task<IEnumerable<ReminderDto>> GetOverdueRemindersAsync(int userId);
        Task<ReminderDto?> GetReminderByIdAsync(int userId, int id);
        Task AddReminderAsync(int userId, ReminderDto reminderDto);
        Task UpdateReminderAsync(int userId, ReminderDto reminderDto);
        Task DeleteReminderAsync(int userId, int id);
        Task MarkAsDoneAsync(int userId, int id);
        Task<int> GetTotalCountAsync(int userId);
        Task<int> GetPendingCountAsync(int userId);
        Task<int> GetCompletedCountAsync(int userId);
    }
}
