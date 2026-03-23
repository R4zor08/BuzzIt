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

        public IEnumerable<Post> GetAll()
        {
            return _context.Posts
                .OrderByDescending(p => p.Priority)
                .ThenBy(p => p.DueDate)
                .ToList();
        }

        public IEnumerable<Post> Search(string? searchTerm, Category? category, Priority? priority, bool? isCompleted)
        {
            var query = _context.Posts.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.Title.Contains(searchTerm) || p.Content.Contains(searchTerm));
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

        public IEnumerable<Post> GetUpcomingPosts(int days = 7)
        {
            var now = DateTime.Now.Date;
            var endDate = now.AddDays(days);

            return _context.Posts
                .Where(p => !p.IsCompleted && p.DueDate.HasValue && p.DueDate.Value.Date >= now && p.DueDate.Value.Date <= endDate)
                .OrderBy(p => p.DueDate)
                .ThenByDescending(p => p.Priority)
                .ToList();
        }

        public IEnumerable<Post> GetOverduePosts()
        {
            var now = DateTime.Now.Date;

            return _context.Posts
                .Where(p => !p.IsCompleted && p.DueDate.HasValue && p.DueDate.Value.Date < now)
                .OrderByDescending(p => p.Priority)
                .ThenBy(p => p.DueDate)
                .ToList();
        }

        public IEnumerable<Post> GetByCategory(Category category)
        {
            return _context.Posts
                .Where(p => p.Category == category)
                .OrderByDescending(p => p.Priority)
                .ThenBy(p => p.DueDate)
                .ToList();
        }

        public IEnumerable<Post> GetByPriority(Priority priority)
        {
            return _context.Posts
                .Where(p => p.Priority == priority)
                .OrderBy(p => p.DueDate)
                .ToList();
        }

        public Post? GetById(int id)
        {
            return _context.Posts.Find(id);
        }

        public void Create(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }

        public void Update(Post post)
        {
            _context.Posts.Update(post);
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var post = _context.Posts.Find(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
        }

        public void MarkAsDone(int id)
        {
            var post = _context.Posts.Find(id);
            if (post != null && !post.IsCompleted)
            {
                post.IsCompleted = true;
                post.CompletedAt = DateTime.Now;
                _context.SaveChanges();
            }
        }

        public int GetTotalCount()
        {
            return _context.Posts.Count();
        }

        public int GetCompletedCount()
        {
            return _context.Posts.Count(p => p.IsCompleted);
        }

        public int GetPendingCount()
        {
            return _context.Posts.Count(p => !p.IsCompleted);
        }
    }
}
