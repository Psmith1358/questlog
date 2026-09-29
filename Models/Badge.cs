using System.ComponentModel.DataAnnotations;

namespace QuestLog.Models;

public enum BadgeRule { TasksCompleted = 0, Streak = 1, LevelReached = 2 }

// Entity 4: Badge. Owner: Person A.
public class Badge
{
    public int Id { get; set; }

    [Required, StringLength(60)]
    public string Name { get; set; } = string.Empty;

    [StringLength(200)]
    public string Description { get; set; } = string.Empty;

    public BadgeRule RuleType { get; set; } = BadgeRule.TasksCompleted;
    public int Threshold { get; set; } = 1;

    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
