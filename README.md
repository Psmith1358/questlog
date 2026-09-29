# QuestLog: Gamified Productivity App

Complete tasks, earn XP, level up, keep streaks, and unlock badges.
Team of 3. Built with ASP.NET Core Razor Pages (.NET 8), Entity Framework Core, SQLite, and ASP.NET Core Identity.

## Quick start (everyone does this once)
Requires the .NET 8 SDK (or newer; if you have a newer SDK, change `net8.0` and the `8.0.*` package versions in `QuestLog.csproj` to match).

```
git clone <repo-url>
cd QuestLog
dotnet tool install --global dotnet-ef     # one time per computer
dotnet run
```
The app creates the database and loads sample data on first run, then prints a local URL to open.
Sample logins: `alex`, `sam`, `jordan`. Password for all: `Test123!`

To reset the sample data: stop the app, delete `questlog.db`, run again.

### One-time setup (ONE person, before anyone else clones)
No migration exists yet. One person runs this, then commits and pushes the new `Migrations` folder:
```
dotnet ef migrations add InitialCreate
dotnet run          # confirm it starts and you can log in as alex
```

## Data model (agreed, do not rename without telling the team)

| # | Entity | Owner | Fields | Relationships |
|---|--------|-------|--------|---------------|
| 1 | **ApplicationUser** | A | UserName, Password (Identity), TotalXp, CurrentStreak, LastCompletedDate, Level (calculated) | Has many Tasks (1-to-many); has many Badges through UserBadge |
| 2 | **Category** | B | Name | Has many Tasks (1-to-many) |
| 3 | **TaskItem** | B | Title, DueDate, Priority (Low/Medium/High), Status (Todo/Done), XpValue, CompletedAt | Belongs to one User and one Category |
| 4 | **Badge** | A | Name, Description, RuleType, Threshold | Has many UserBadges |
| 5 | **UserBadge** | A | UserId, BadgeId, EarnedAt | Join table (many-to-many User and Badge) |

Rubric check: 5 entities (min 4), all related, User has multiple related entities, 1-to-many relationships (User-Task, Category-Task), authentication via Identity.

## Rules the team agreed to
**Naming**
- Classes are singular and PascalCase (`TaskItem`). Properties are PascalCase (`DueDate`).
- The task class is `TaskItem` (not `Task`) to avoid clashing with .NET's built-in `Task`.
- XP by priority: Low 10, Medium 25, High 50 (`TaskItem.XpForPriority`).
- Level formula: `Level = 1 + TotalXp / 100` (calculated on `ApplicationUser`, not stored).
- Changing a model or property name needs a message to the group first.

**Ownership (avoids merge conflicts)**
- Person A: user accounts and badges. `Badge`, `UserBadge`, `ApplicationUser`, login/register pages, badge unlock logic, profile page.
- Person B: `TaskItem`, `Category`, task pages (create/view/edit/delete), search page.
- Person C: XP, level, streak logic, custom stats page, shared layout (`Pages/Shared`), navigation, and styling.
- Each person adds their own folder under `Pages/` (for example `Pages/Tasks/` and `Pages/Stats/`). Every page is a pair of files: `Something.cshtml` (the HTML) and `Something.cshtml.cs` (the code).
- Tip: Visual Studio can scaffold Create/Details/Edit/Delete/Index pages for a model in one step (right-click `Pages` > Add > New Scaffolded Item > Razor Pages using Entity Framework (CRUD)).
- If you need a change in someone else's files, ask them or open a small PR.

**Git workflow**
- Never commit directly to `main`.
- One branch per person: `feature/a-accounts`, `feature/b-tasks`, `feature/c-stats`.
- Open a pull request into `main`; a teammate glances at it and merges. Merge small and often.
- Run `git pull origin main` at the start of each work session.
- Model changes: after editing a model, run `dotnet ef migrations add <ShortDescription>` and commit the migration files. If two people add migrations at once, tell the group before merging; the fix is to delete one branch's migration and recreate it after merging.
- Never commit `questlog.db` (already in `.gitignore`).

## Project layout
```
Models/        entity classes
Data/          ApplicationDbContext and SeedData
Pages/         Razor Pages (one folder per feature; Shared/ holds layout and nav)
Program.cs     app setup, Identity, database, seed on startup
```
