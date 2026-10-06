using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestLog.Data;
using QuestLog.Models;

namespace QuestLog.Pages.Profile;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    // The profile information for the currently-signed-in user
    public ApplicationUser ProfileUser { get; set; } = default!;

    public async Task<IActionResult> OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            // Not signed in
            return Challenge();
        }

        // Load the user and related data so the page can display tasks and badges
        ProfileUser = await _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.Tasks)
            .Include(u => u.UserBadges)
                .ThenInclude(ub => ub.Badge)
            .FirstOrDefaultAsync() ?? null!;

        if (ProfileUser == null)
        {
            return NotFound();
        }

        return Page();
    }
}
