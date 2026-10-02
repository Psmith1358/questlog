using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using QuestLog.Data;

namespace QuestLog.Pages.Stats;

public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _context;

    public IndexModel(ApplicationDbContext context)
    {
        _context = context;
    }

    public int TotalXp { get; set; }
    public int Level { get; set; }
    public int XpIntoLevel { get; set; }
    public int XpNeededForNextLevel { get; set; }

    public async Task OnGetAsync()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return;
        }

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            return;
        }

        TotalXp = user.TotalXp;
        Level = user.Level;
        XpIntoLevel = user.TotalXp % 100;
        XpNeededForNextLevel = 100 - XpIntoLevel;
    }
}