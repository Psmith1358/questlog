using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestLog.Models;
using QuestLog.Data;

namespace QuestLog.Pages.Tasks;

public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public CreateModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult OnGet()
    {
        ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
        return Page();
    }

    [BindProperty]
    public TaskItem TaskItem { get; set; } = default!;

    // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        TaskItem.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        TaskItem.XpValue = TaskItem.XpForPriority(TaskItem.Priority);
        TaskItem.Status = QuestLog.Models.TaskStatus.Todo;
        TaskItem.CompletedAt = null;

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

        _context.Tasks.Add(TaskItem);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
