namespace QuestLog.Models;

// Entity 5: UserBadge (join table, many-to-many User <-> Badge). Owner: Person A.
public class UserBadge
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int BadgeId { get; set; }
    public Badge? Badge { get; set; }

    public DateTime EarnedAt { get; set; } = DateTime.UtcNow;
}
