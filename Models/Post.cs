using System.ComponentModel.DataAnnotations;

namespace BuzzIt.Models
{
    public enum Category
    {
        General = 0,
        Work = 1,
        Personal = 2,
        Ideas = 3,
        Urgent = 4
    }

    public enum Priority
    {
        Low = 0,
        Medium = 1,
        High = 2
    }

    public class Post
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public bool IsCompleted { get; set; } = false;

        public DateTime? CompletedAt { get; set; }

        public Category Category { get; set; } = Category.General;

        public Priority Priority { get; set; } = Priority.Medium;

        [Display(Name = "Due Date")]
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
    }
}
