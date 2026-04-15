using System;
using System.ComponentModel.DataAnnotations;

namespace BuzzIt.Models
{
    public class Reminder
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public ApplicationUser? User { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        public DateTime Time { get; set; }

        public string Description { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public Category Category { get; set; } = Category.General;

        public Priority Priority { get; set; } = Priority.Medium;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }
}