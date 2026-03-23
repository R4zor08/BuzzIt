using System.ComponentModel.DataAnnotations;
using BuzzIt.Models;

namespace BuzzIt.Requests
{
    /// <summary>
    /// Request pattern for creating a new post
    /// </summary>
    public class CreatePostRequest : BaseRequest
    {
        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

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

            if (!ValidateRequired(Content, "Content"))
                return false;

            if (DueDate.HasValue && DueDate.Value.Date < DateTime.Now.Date)
            {
                AddError("DueDate", "Due date cannot be in the past.");
                return false;
            }

            return true;
        }

        public Post ToPost()
        {
            return new Post
            {
                Title = Title,
                Content = Content,
                Category = Category,
                Priority = Priority,
                DueDate = DueDate
            };
        }
    }

    /// <summary>
    /// Request pattern for updating an existing post
    /// </summary>
    public class UpdatePostRequest : BaseRequest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 1)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

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
                AddError("Id", "Invalid post ID.");
                return false;
            }

            if (!ValidateRequired(Title, "Title"))
                return false;

            if (!ValidateMaxLength(Title, 100, "Title"))
                return false;

            if (!ValidateRequired(Content, "Content"))
                return false;

            return true;
        }

        public Post ToPost()
        {
            return new Post
            {
                Id = Id,
                Title = Title,
                Content = Content,
                Category = Category,
                Priority = Priority,
                DueDate = DueDate,
                IsCompleted = IsCompleted,
                CompletedAt = CompletedAt
            };
        }
    }

    /// <summary>
    /// Request pattern for searching/filtering posts
    /// </summary>
    public class SearchPostRequest : BaseRequest
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
    /// Request pattern for marking a post as done
    /// </summary>
    public class MarkPostDoneRequest : BaseRequest
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            ClearErrors();

            if (Id <= 0)
            {
                AddError("Id", "Invalid post ID.");
                return false;
            }

            return true;
        }
    }

    /// <summary>
    /// Request pattern for deleting a post
    /// </summary>
    public class DeletePostRequest : BaseRequest
    {
        public int Id { get; set; }

        public override bool IsValid()
        {
            ClearErrors();

            if (Id <= 0)
            {
                AddError("Id", "Invalid post ID.");
                return false;
            }

            return true;
        }
    }
}
