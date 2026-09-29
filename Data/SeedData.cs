using Microsoft.AspNetCore.Identity;
using QuestLog.Models;
using TaskStatus = QuestLog.Models.TaskStatus;

namespace QuestLog.Data;

// Loads sample data the first time the app runs against an empty database.
// To reload: stop the app, delete questlog.db, and run again.
// Sample users: alex, sam, jordan. Password for all: Test123!
public static class SeedData
{
    private static readonly string[] Titles =
    {
        "Finish report", "Read chapter", "Go for a run", "Clean garage", "Call the dentist",
        "Plan meals", "Pay bills", "Study for quiz", "Update resume", "Meal prep", "Team meeting prep"
    };

    public static async Task InitializeAsync(IServiceProvider services)
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (db.Categories.Any()) return;

        var categories = new[] { "Work", "School", "Health", "Home", "Personal" }
            .Select(n => new Category { Name = n }).ToList();
        db.Categories.AddRange(categories);

        var badges = new List<Badge>
        {
            new() { Name = "First Step", Description = "Complete your first task", RuleType = BadgeRule.TasksCompleted, Threshold = 1 },
            new() { Name = "Getting Going", Description = "Complete 10 tasks", RuleType = BadgeRule.TasksCompleted, Threshold = 10 },
            new() { Name = "Task Master", Description = "Complete 50 tasks", RuleType = BadgeRule.TasksCompleted, Threshold = 50 },
            new() { Name = "On a Roll", Description = "Reach a 3-day streak", RuleType = BadgeRule.Streak, Threshold = 3 },
            new() { Name = "Level 5", Description = "Reach level 5", RuleType = BadgeRule.LevelReached, Threshold = 5 },
        };
        db.Badges.AddRange(badges);
        await db.SaveChangesAsync();

        var rng = new Random(42);
        var now = DateTime.UtcNow;

        foreach (var name in new[] { "alex", "sam", "jordan" })
        {
            var user = new ApplicationUser
            {
                UserName = $"{name}@example.com",
                Email = $"{name}@example.com",
                EmailConfirmed = true,
                CurrentStreak = rng.Next(0, 6)
            };
            var result = await userManager.CreateAsync(user, "Test123!");
            if (!result.Succeeded) throw new Exception("Seed user failed: " + string.Join("; ", result.Errors.Select(e => e.Description)));

            int doneCount = 0;
            int xp = 0;
            for (int i = 0; i < 15; i++)
            {
                var priority = (Priority)rng.Next(0, 3);
                bool done = rng.NextDouble() < 0.6;
                var task = new TaskItem
                {
                    UserId = user.Id,
                    Category = categories[rng.Next(categories.Count)],
                    Title = Titles[rng.Next(Titles.Length)],
                    Priority = priority,
                    XpValue = TaskItem.XpForPriority(priority),
                    DueDate = now.Date.AddDays(rng.Next(-5, 15)),
                    Status = done ? TaskStatus.Done : TaskStatus.Todo,
                    CompletedAt = done ? now.AddDays(-rng.Next(0, 21)) : null
                };
                if (done) { doneCount++; xp += task.XpValue; }
                db.Tasks.Add(task);
            }
            user.TotalXp = xp;
            if (doneCount >= 1) db.UserBadges.Add(new UserBadge { UserId = user.Id, Badge = badges[0] });
            await userManager.UpdateAsync(user);
        }
        await db.SaveChangesAsync();
    }
}
