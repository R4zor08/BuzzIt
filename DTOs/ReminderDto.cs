using System;
using BuzzIt.Models;

namespace BuzzIt.DTOs
{
    public class ReminderDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime Time { get; set; }
        public string Description { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public Category Category { get; set; }
        public Priority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}