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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(DashboardViewModel vm)
    {
        // Detailed Debugging
        Console.WriteLine("=== Add Method Called ===");
        Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");
        Console.WriteLine($"vm is null: {vm is null}");
        Console.WriteLine($"vm.NewGuest is null: {vm?.NewGuest is null}");
        
        if (vm?.NewGuest != null)
        {
            Console.WriteLine($"Name from form: '{vm.NewGuest.Name}'");
            Console.WriteLine($"Purpose from form: '{vm.NewGuest.Purpose}'");
        }

        // If ModelState is invalid, show the specific errors
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            var errorMessage = string.Join("; ", errors);
            TempData["Error"] = $"Validation failed: {errorMessage}";
            Console.WriteLine($"ModelState Errors: {errorMessage}");
            return RedirectToAction(nameof(Index));
        }

        // Check if NewGuest itself is null (a common problem)
        if (vm?.NewGuest == null)
        {
            TempData["Error"] = "Form data error: NewGuest is null. Please check the form structure.";
            Console.WriteLine("Error: vm.NewGuest is null.");
            return RedirectToAction(nameof(Index));
        }

        // Check if required fields are empty
        if (string.IsNullOrWhiteSpace(vm.NewGuest.Name))
        {
            TempData["Error"] = "Name is required.";
            return RedirectToAction(nameof(Index));
        }

        if (string.IsNullOrWhiteSpace(vm.NewGuest.Purpose))
        {
            TempData["Error"] = "Purpose is required.";
            return RedirectToAction(nameof(Index));
        }

        var userId = _userManager.GetUserId(User);
        if (string.IsNullOrEmpty(userId))
        {
            TempData["Error"] = "You must be logged in to add a guest.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            var entry = new GuestEntry
            {
                Name = vm.NewGuest.Name.Trim(),
                Purpose = vm.NewGuest.Purpose.Trim(),
                TimeIn = DateTime.UtcNow,
                LoggedByUserId = userId,
                LoggedAt = DateTime.UtcNow
            };

            _db.GuestEntries.Add(entry);
            await _db.SaveChangesAsync();

            TempData["Success"] = "Guest added successfully!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Database error: {ex.Message}";
            Console.WriteLine($"Database error: {ex.Message}");
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TimeOut(int id)
    {
        try
        {
            var entry = await _db.GuestEntries.FindAsync(id);
            if (entry != null && entry.TimeOut == null)
            {
                entry.TimeOut = DateTime.UtcNow;
                await _db.SaveChangesAsync();
                TempData["Success"] = "Guest checked out successfully!";
            }
            else
            {
                TempData["Error"] = "Guest already checked out or not found.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var entry = await _db.GuestEntries.FindAsync(id);
            if (entry != null)
            {
                _db.GuestEntries.Remove(entry);
                await _db.SaveChangesAsync();
                TempData["Success"] = "Guest entry deleted successfully!";
            }
            else
            {
                TempData["Error"] = "Guest entry not found.";
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}