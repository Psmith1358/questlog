using Microsoft.EntityFrameworkCore;
using QuestLog.Data;
using QuestLog.Models;

namespace QuestLog.Services;

public class GamificationService
{
    private readonly ApplicationDbContext _context;

    public GamificationService(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<bool> CompleteTaskAsync(int taskId, string userId)
{
    var task = await _context.Tasks
        .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

    if (task == null || task.Status == QuestLog.Models.TaskStatus.Done)
    {
        return false;
    }

    var user = await _context.Users
        .FirstOrDefaultAsync(u => u.Id == userId);

    if (user == null)
    {
        return false;
    }

    task.Status = QuestLog.Models.TaskStatus.Done;
    task.CompletedAt = DateTime.Now;

    user.TotalXp += task.XpValue;

    var today = DateTime.Today;

if (user.LastCompletedDate == null)
{
    user.CurrentStreak = 1;
}
else
{
    var lastCompleted = user.LastCompletedDate.Value.Date;

    if (lastCompleted == today.AddDays(-1))
    {
        user.CurrentStreak++;
    }
    else if (lastCompleted < today.AddDays(-1))
    {
        user.CurrentStreak = 1;
    }
}

user.LastCompletedDate = today;

await _context.SaveChangesAsync();

await CheckBadgesAsync(user);

await _context.SaveChangesAsync();

return true;
}
private async Task CheckBadgesAsync(ApplicationUser user)
{
    var badges = await _context.Badges.ToListAsync();

    var earnedBadgeIds = await _context.UserBadges
        .Where(ub => ub.UserId == user.Id)
        .Select(ub => ub.BadgeId)
        .ToListAsync();

    var completedTaskCount = await _context.Tasks
        .CountAsync(t =>
            t.UserId == user.Id &&
            t.Status == QuestLog.Models.TaskStatus.Done);

    foreach (var badge in badges)
    {
        if (earnedBadgeIds.Contains(badge.Id))
        {
            continue;
        }

        bool earned = badge.RuleType switch
        {
            BadgeRule.TasksCompleted =>
                completedTaskCount >= badge.Threshold,

            BadgeRule.Streak =>
                user.CurrentStreak >= badge.Threshold,

            BadgeRule.LevelReached =>
                user.Level >= badge.Threshold,

            _ => false
        };

        if (earned)
        {
            _context.UserBadges.Add(new UserBadge
            {
                UserId = user.Id,
                BadgeId = badge.Id
            });
        }
    }
}
}