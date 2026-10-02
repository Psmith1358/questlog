using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestLog.Data;

namespace QuestLog.Pages.Stats;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalXp { get; set; }
    public int Level { get; set; }
    public int XpIntoLevel { get; set; }
    public int XpNeededForNextLevel { get; set; }

    public int CurrentStreak { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public double CompletionRate { get; set; }

    public List<CategoryStat> CategoryStats { get; set; } = new();
    public List<DailyXpStat> DailyXpStats { get; set; } = new();
    public List<QuestLog.Models.Badge> EarnedBadges { get; set; } = new();

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return;
        }

        TotalXp = user.TotalXp;
        Level = user.Level;
        XpIntoLevel = user.TotalXp % 100;
        XpNeededForNextLevel = 100 - XpIntoLevel;

        CurrentStreak = user.CurrentStreak;

        var userTasks = await _context.Tasks
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .ToListAsync();

        TotalTasks = userTasks.Count;

        CompletedTasks = userTasks.Count(
            t => t.Status == QuestLog.Models.TaskStatus.Done);

        CompletionRate = TotalTasks == 0
            ? 0
            : (double)CompletedTasks / TotalTasks * 100;

        CategoryStats = userTasks
            .GroupBy(t => t.Category?.Name ?? "Uncategorized")
            .Select(group => new CategoryStat
            {
                CategoryName = group.Key,
                TotalTasks = group.Count(),
                CompletedTasks = group.Count(
                    t => t.Status == QuestLog.Models.TaskStatus.Done)
            })
            .OrderBy(stat => stat.CategoryName)
            .ToList();

        DailyXpStats = userTasks
            .Where(t =>
                t.Status == QuestLog.Models.TaskStatus.Done &&
                t.CompletedAt.HasValue)
            .GroupBy(t => t.CompletedAt!.Value.Date)
            .Select(group => new DailyXpStat
            {
                Date = group.Key,
                XpEarned = group.Sum(t => t.XpValue)
            })
            .OrderBy(stat => stat.Date)
            .ToList();

        EarnedBadges = await _context.UserBadges
            .Where(ub => ub.UserId == userId)
            .Include(ub => ub.Badge)
            .Where(ub => ub.Badge != null)
            .Select(ub => ub.Badge!)
            .OrderBy(b => b.Name)
            .ToListAsync();  
        }
}

   public class CategoryStat
{
    public string CategoryName { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }

    public double CompletionRate =>
        TotalTasks == 0
            ? 0
            : (double)CompletedTasks / TotalTasks * 100;
}

    public class DailyXpStat
{
    public DateTime Date { get; set; }
    public int XpEarned { get; set; }
}