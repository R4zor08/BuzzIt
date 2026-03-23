using Microsoft.EntityFrameworkCore;
using BuzzIt.Models;

namespace BuzzIt.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Reminder> Reminders { get; set; } = default!;
        public DbSet<Post> Posts { get; set; } = default!;
    }
}