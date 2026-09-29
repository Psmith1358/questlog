using System.ComponentModel.DataAnnotations;

namespace QuestLog.Models;

// Entity 2: Category. Owner: Person B. Category has many Tasks (1-to-many).
public class Category
{
    public int Id { get; set; }

    [Required, StringLength(50)]
    public string Name { get; set; } = string.Empty;

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
