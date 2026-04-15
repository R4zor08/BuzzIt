namespace BuzzIt.Models;

public class DashboardViewModel
{
    public int TotalPosts { get; set; }
    public int PendingPosts { get; set; }
    public int CompletedPosts { get; set; }
    public int TotalReminders { get; set; }
    public int PendingReminders { get; set; }
    public int CompletedReminders { get; set; }
}
