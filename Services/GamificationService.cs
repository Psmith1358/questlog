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

    return true;
}
}