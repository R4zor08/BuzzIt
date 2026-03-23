using BuzzIt.Models;

namespace BuzzIt.Services.Interfaces
{
    public interface IPostService
    {
        IEnumerable<Post> GetAll();
        IEnumerable<Post> Search(string? searchTerm, Category? category, Priority? priority, bool? isCompleted);
        IEnumerable<Post> GetUpcomingPosts(int days = 7);
        IEnumerable<Post> GetOverduePosts();
        IEnumerable<Post> GetByCategory(Category category);
        IEnumerable<Post> GetByPriority(Priority priority);
        Post? GetById(int id);
        void Create(Post post);
        void Update(Post post);
        void Delete(int id);
        void MarkAsDone(int id);
        int GetTotalCount();
        int GetCompletedCount();
        int GetPendingCount();
    }
}
