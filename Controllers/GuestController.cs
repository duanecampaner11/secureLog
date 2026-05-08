using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureLog.Data;
using SecureLog.Models;

namespace SecureLog.Controllers;

[Authorize]
public class GuestController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public GuestController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search)
    {
        var query = _db.GuestEntries.AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var s = search.Trim();
            if (DateTime.TryParse(s, out var date))
            {
                var d = date.Date;
                query = query.Where(g => g.TimeIn.Date == d);
            }
            else
            {
                query = query.Where(g => g.Name.Contains(s) || g.Purpose.Contains(s));
            }
        }

        var entries = await query.OrderByDescending(g => g.TimeIn).Take(500).ToListAsync();

        var userIds = entries.Select(e => e.LoggedByUserId).Distinct().ToList();
        var users = await _db.Users.Where(u => userIds.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName }).ToListAsync();

        var vm = new DashboardViewModel
        {
            Entries = entries,
            Search = search,
            UserNames = users.ToDictionary(u => u.Id, u => u.UserName ?? "(unknown)")
        };
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(DashboardViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please enter a visitor name and purpose.";
            return RedirectToAction(nameof(Index));
        }

        var userId = _userManager.GetUserId(User)!;
        var entry = new GuestEntry
        {
            Name = vm.NewGuest.Name.Trim(),
            Purpose = vm.NewGuest.Purpose.Trim(),
            TimeIn = DateTime.UtcNow,        // CHANGED: Now using UtcNow
            LoggedByUserId = userId,
            LoggedAt = DateTime.UtcNow
        };
        _db.GuestEntries.Add(entry);
        await _db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> TimeOut(int id)
    {
        var entry = await _db.GuestEntries.FindAsync(id);
        if (entry != null && entry.TimeOut == null)
        {
            entry.TimeOut = DateTime.UtcNow;  // CHANGED: Now using UtcNow
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var entry = await _db.GuestEntries.FindAsync(id);
        if (entry != null)
        {
            _db.GuestEntries.Remove(entry);
            await _db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}