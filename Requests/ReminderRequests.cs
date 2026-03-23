using System.ComponentModel.DataAnnotations;
using BuzzIt.Models;

namespace BuzzIt.Requests
{
    /// <summary>
    /// Request pattern for creating a new reminder
    /// </summary>
    public class CreateReminderRequest : BaseRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime Time { get; set; }

        public string Description { get; set; } = string.Empty;

        public Category Category { get; set; } = Category.General;

        public Priority Priority { get; set; } = Priority.Medium;

        public DateTime? DueDate { get; set; }

        public override bool IsValid()
        {
            ClearErrors();

            if (!ValidateRequired(Title, "Title"))
                return false;

            if (!ValidateMaxLength(Title, 100, "Title"))
                return false;

            if (Time == default)
            {
                AddError("Time", "Time is required.");
                return false;
            }

            return true;
        }

        public Reminder ToReminder()
        {
            return new Reminder
            {
                Title = Title,
                Time = Time,
                Description = Description,
                Category = Category,
                Priority = Priority,
                DueDate = DueDate
            };
        }
    }

    /// <summary>
    /// Request pattern for updating an existing reminder
    /// </summary>
    public class UpdateReminderRequest : BaseRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public DateTime Time { get; set; }

        public string Description { get; set; } = string.Empty;

        public Category Category { get; set; } = Category.General;

        public Priority Priority { get; set; } = Priority.Medium;

        public DateTime? DueDate { get; set; }

        public bool IsCompleted { get; set; }

        public DateTime? CompletedAt { get; set; }

        public override bool IsValid()
        {
            ClearErrors();

            if (Id <= 0)
            {
                AddError("Id", "Invalid reminder ID.");
                return false;
            }

            if (!ValidateRequired(Title, "Title"))
                return false;

            if (!ValidateMaxLength(Title, 100, "Title"))
                return false;

            if (Time == default)
            {
                AddError("Time", "Time is required.");
                return false;
            }

            return true;
        }

        public Reminder ToReminder()
        {
            return new Reminder
            {
                Id = Id,
                Title = Title,
                Time = Time,
                Description = Description,
                Category = Category,
                Priority = Priority,
                DueDate = DueDate,
                IsCompleted = IsCompleted,
                CompletedAt = CompletedAt
            };
        }
    }

    /// <summary>
    /// Request pattern for searching/filtering reminders
    /// </summary>
    public class SearchReminderRequest : BaseRequest
    {
        public string? SearchTerm { get; set; }

        public Category? Category { get; set; }

        public Priority? Priority { get; set; }

        public bool? IsCompleted { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public override bool IsValid()
        {
            ClearErrors();

            if (Page < 1)
            {
                AddError("Page", "Page must be greater than 0.");
                return false;
            }

            if (PageSize < 1 || PageSize > 100)
            {
                AddError("PageSize", "PageSize must be between 1 and 100.");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Request pattern for marking a reminder as done
    /// </summary>
    public class MarkReminderDoneRequest : BaseRequest
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            ClearErrors();

            if (Id <= 0)
            {
                AddError("Id", "Invalid reminder ID.");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Request pattern for deleting a reminder
    /// </summary>
    public class DeleteReminderRequest : BaseRequest
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            ClearErrors();

            if (Id <= 0)
            {
                AddError("Id", "Invalid reminder ID.");
                return false;
            }

            return true;
        }
    }
}
