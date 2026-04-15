using BuzzIt.Data;
using BuzzIt.Models;
using BuzzIt.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BuzzIt.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Post> GetAll(int userId)
        {
            return BaseQuery(userId)
                .OrderByDescending(p => p.Priority)
                .ThenBy(p => p.DueDate)
                .ToList();
        }

        public IEnumerable<Post> Search(int userId, string? searchTerm, Category? category, Priority? priority, bool? isCompleted)
        {
            var query = BaseQuery(userId);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLowerInvariant();
                query = query.Where(p =>
                    p.Title.ToLower().Contains(term) ||
                    p.Content.ToLower().Contains(term));
            }

            if (category.HasValue)
            {
                query = query.Where(p => p.Category == category.Value);
            }

            if (priority.HasValue)
            {
                query = query.Where(p => p.Priority == priority.Value);
            }

            if (isCompleted.HasValue)
            {
                query = query.Where(p => p.IsCompleted == isCompleted.Value);
            }

            return query.OrderByDescending(p => p.Priority).ThenBy(p => p.DueDate).ToList();
        }

        public IEnumerable<Post> GetUpcomingPosts(int userId, int days = 7)
        {
            var now = DateTime.Now.Date;
            var endDate = now.AddDays(days);

            return BaseQuery(userId)
                .Where(p => !p.IsCompleted && p.DueDate.HasValue && p.DueDate.Value.Date >= now && p.DueDate.Value.Date <= endDate)
                .OrderBy(p => p.DueDate)
                .ThenByDescending(p => p.Priority)
                .ToList();
        }

        public IEnumerable<Post> GetOverduePosts(int userId)
        {
            var now = DateTime.Now.Date;

            return BaseQuery(userId)
                .Where(p => !p.IsCompleted && p.DueDate.HasValue && p.DueDate.Value.Date < now)
                .OrderByDescending(p => p.Priority)
                .ThenBy(p => p.DueDate)
                .ToList();
        }

        public IEnumerable<Post> GetByCategory(int userId, Category category)
        {
            return BaseQuery(userId)
                .Where(p => p.Category == category)
                .OrderByDescending(p => p.Priority)
                .ThenBy(p => p.DueDate)
                .ToList();
        }

        public IEnumerable<Post> GetByPriority(int userId, Priority priority)
        {
            return BaseQuery(userId)
                .Where(p => p.Priority == priority)
                .OrderBy(p => p.DueDate)
                .ToList();
        }

        public Post? GetById(int userId, int id)
        {
            return BaseQuery(userId).FirstOrDefault(p => p.Id == id);
        }

        public void Create(int userId, Post post)
        {
            post.UserId = userId;
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void Update(int userId, Post post)
        {
            var existing = _context.Posts.FirstOrDefault(p => p.Id == post.Id && p.UserId == userId);
            if (existing == null)
            {
                throw new InvalidOperationException("Post not found.");
            }

            existing.Title = post.Title;
            existing.Content = post.Content;
            existing.Category = post.Category;
            existing.Priority = post.Priority;
            existing.DueDate = post.DueDate;
            existing.IsCompleted = post.IsCompleted;
            existing.CompletedAt = post.CompletedAt;
            _context.SaveChanges();
        }

        public void Delete(int userId, int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id && p.UserId == userId);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
        }

        public void MarkAsDone(int userId, int id)
        {
            var post = _context.Posts.FirstOrDefault(p => p.Id == id && p.UserId == userId);
            if (post != null && !post.IsCompleted)
            {
                post.IsCompleted = true;
                post.CompletedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public int GetTotalCount(int userId)
        {
            return _context.Posts.Count(p => p.UserId == userId);
        }

        public int GetCompletedCount(int userId)
        {
            return _context.Posts.Count(p => p.UserId == userId && p.IsCompleted);
        }

        public int GetPendingCount(int userId)
        {
            return _context.Posts.Count(p => p.UserId == userId && !p.IsCompleted);
        }

        private IQueryable<Post> BaseQuery(int userId) =>
            _context.Posts.Where(p => p.UserId == userId);
    }
}
