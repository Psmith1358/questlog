using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace QuestLog.Models;

// Entity 1: User. Owner: Person A. Login fields come from IdentityUser.
public class ApplicationUser : IdentityUser
{
    public int TotalXp { get; set; }
    public int CurrentStreak { get; set; }
    public DateTime? LastCompletedDate { get; set; }

    // Agreed formula: level = 1 + TotalXp / 100 (calculated, not stored)
    [NotMapped]
    public int Level => 1 + TotalXp / 100;

    // User has many Tasks (1-to-many); User has many Badges through UserBadge
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    public ICollection<UserBadge> UserBadges { get; set; } = new List<UserBadge>();
}
