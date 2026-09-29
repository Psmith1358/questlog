using System.ComponentModel.DataAnnotations;

namespace QuestLog.Models;

public enum Priority { Low = 0, Medium = 1, High = 2 }
public enum TaskStatus { Todo = 0, Done = 1 }

// Entity 3: Task. Owner: Person B. Named TaskItem to avoid clashing with System.Threading.Tasks.Task.
public class TaskItem
{
    public int Id { get; set; }

    [Required, StringLength(120)]
    public string Title { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    public DateTime? DueDate { get; set; }

    public Priority Priority { get; set; } = Priority.Medium;
    public TaskStatus Status { get; set; } = TaskStatus.Todo;
    public int XpValue { get; set; } = 25;
    public DateTime? CompletedAt { get; set; }

    // Belongs to one User and one Category
    [Required]
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    // Agreed XP by priority: Low 10, Medium 25, High 50
    public static int XpForPriority(Priority p) => p switch
    {
        Priority.Low => 10,
        Priority.Medium => 25,
        Priority.High => 50,
        _ => 25
    };
}
