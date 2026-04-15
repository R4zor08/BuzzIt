using BuzzIt.Models;

namespace BuzzIt.Services.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetAll(int userId);
        IEnumerable<Post> Search(int userId, string? searchTerm, Category? category, Priority? priority, bool? isCompleted);
        IEnumerable<Post> GetUpcomingPosts(int userId, int days = 7);
        IEnumerable<Post> GetOverduePosts(int userId);
        IEnumerable<Post> GetByCategory(int userId, Category category);
        IEnumerable<Post> GetByPriority(int userId, Priority priority);
        Post? GetById(int userId, int id);
        void Create(int userId, Post post);
        void Update(int userId, Post post);
        void Delete(int userId, int id);
        void MarkAsDone(int userId, int id);
        int GetTotalCount(int userId);
        int GetCompletedCount(int userId);
        int GetPendingCount(int userId);
    }
}
