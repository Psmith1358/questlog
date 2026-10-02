using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestLog.Models;
using QuestLog.Data;
using System.ComponentModel;

namespace QuestLog.Pages.Tasks;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public IList<TaskItem> TaskItem { get; set; } = default!;

    [BindProperty(SupportsGet = true)]
    public string SearchString { get; set; } = string.Empty;
    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var tasks = _context.Tasks
            .Where(t => t.UserId == userId)
            .Include(t => t.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(SearchString))
        {
            tasks = tasks.Where(t => t.Title.Contains(SearchString));
        }

        TaskItem = await tasks.ToListAsync();
    }
    public async Task<IActionResult> OnPostMarkCompleteAsync(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var task = await _context.Tasks
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (task is null)
        {
            return NotFound();
        }

        task.Status = QuestLog.Models.TaskStatus.Done;
        task.CompletedAt = DateTime.Now;

        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
