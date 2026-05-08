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
    // DEBUG: Check if NewGuest is null
    if (vm.NewGuest == null)
    {
        TempData["Error"] = "Debug: NewGuest is null";
        return RedirectToAction(nameof(Index));
    }
    
    // DEBUG: Check ModelState errors
    if (!ModelState.IsValid)
    {
        var errors = string.Join("; ", ModelState.Values
            .SelectMany(x => x.Errors)
            .Select(x => x.ErrorMessage));
        TempData["Error"] = $"Validation failed: {errors}";
        return RedirectToAction(nameof(Index));
    }
    
    // DEBUG: Check if fields are empty
    if (string.IsNullOrWhiteSpace(vm.NewGuest.Name))
    {
        TempData["Error"] = "Debug: Name is empty";
        return RedirectToAction(nameof(Index));
    }
    
    if (string.IsNullOrWhiteSpace(vm.NewGuest.Purpose))
    {
        TempData["Error"] = "Debug: Purpose is empty";
        return RedirectToAction(nameof(Index));
    }

    var userId = _userManager.GetUserId(User);
    
    // DEBUG: Check if user is logged in
    if (string.IsNullOrEmpty(userId))
    {
        TempData["Error"] = "Debug: User not logged in";
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
        return RedirectToAction(nameof(Index));
    }
}}