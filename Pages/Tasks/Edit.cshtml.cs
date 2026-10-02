using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuestLog.Models;
using QuestLog.Data;
using System.Security.Claims;

namespace QuestLog.Pages.Tasks;

public class EditModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public EditModel(ApplicationDbContext context)
    {
        _context = context;
    }

    [BindProperty]
    public TaskItem TaskItem { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var taskitem = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (taskitem is null)
        {
            return NotFound();
        }

        TaskItem = taskitem;

        ViewData["CategoryId"] = new SelectList(
            _context.Categories,
            "Id",
            "Name",
            TaskItem.CategoryId);

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ModelState.Remove("TaskItem.UserId");

        if (!ModelState.IsValid)
        {
            ViewData["CategoryId"] = new SelectList(
                _context.Categories,
                "Id",
                "Name",
                TaskItem.CategoryId);

            return Page();
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var existingTask = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == TaskItem.Id && t.UserId == userId);

        if (existingTask is null)
        {
            return NotFound();
        }

        existingTask.Title = TaskItem.Title;
        existingTask.DueDate = TaskItem.DueDate;
        existingTask.Priority = TaskItem.Priority;
        existingTask.CategoryId = TaskItem.CategoryId;

        existingTask.XpValue =
            QuestLog.Models.TaskItem.XpForPriority(TaskItem.Priority);

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
