using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestLog.Models;
using QuestLog.Data;
using System.ComponentModel;
using QuestLog.Services;

namespace QuestLog.Pages.Tasks;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;
    private readonly GamificationService _gamification;

    public IndexModel(
        ApplicationDbContext context,
        GamificationService gamification)
    {
    _context = context;
    _gamification = gamification;
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

        if (userId == null)
        {
            return Challenge();
        }

        var completed = await _gamification.CompleteTaskAsync(id, userId);

        if (!completed)
        {
            return NotFound();
        }
        return RedirectToPage("./Index");
    }
}
