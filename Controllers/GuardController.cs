using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureLog.Data;
using SecureLog.Models;

namespace SecureLog.Controllers;

[Authorize(Roles = "Guard")]
public class GuardController : Controller
{
    private readonly ApplicationDbContext _db;

    public GuardController(ApplicationDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Dashboard()
    {
        var today = DateTime.UtcNow.Date;
        // Show today's visits AND future visits (upcoming 7 days)
        var visits = await _db.VisitRequests
            .Where(v => v.VisitDate.Date >= today && 
                       (v.Status == RequestStatus.Confirmed || v.Status == RequestStatus.CheckedIn))
            .OrderBy(v => v.VisitDate)
            .Take(50)
            .ToListAsync();
        return View(visits);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyVisitor(string confirmationId)
    {
        if (string.IsNullOrWhiteSpace(confirmationId))
        {
            TempData["Error"] = "Please enter a confirmation ID";
            return RedirectToAction(nameof(Dashboard));
        }

        var visit = await _db.VisitRequests
            .FirstOrDefaultAsync(v => v.ConfirmationId == confirmationId.ToUpper());

        if (visit == null)
        {
            TempData["Error"] = "Invalid confirmation ID";
            return RedirectToAction(nameof(Dashboard));
        }

        if (visit.Status != RequestStatus.Confirmed && visit.Status != RequestStatus.CheckedIn)
        {
            TempData["Error"] = $"This request is {visit.Status}. Access denied.";
            return RedirectToAction(nameof(Dashboard));
        }

        // Check if visit date is today or future
        if (visit.VisitDate.Date < DateTime.UtcNow.Date)
        {
            TempData["Error"] = $"This confirmation expired on {visit.VisitDate:yyyy-MM-dd}";
            return RedirectToAction(nameof(Dashboard));
        }

        TempData["Success"] = $"Valid visitor: {visit.FullName} - {visit.Purpose} (Date: {visit.VisitDate:yyyy-MM-dd})";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(int id)
    {
        var visit = await _db.VisitRequests.FindAsync(id);
        if (visit != null && visit.Status == RequestStatus.Confirmed)
        {
            visit.Status = RequestStatus.CheckedIn;
            visit.CheckInTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"{visit.FullName} checked in.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(int id)
    {
        var visit = await _db.VisitRequests.FindAsync(id);
        if (visit != null && visit.Status == RequestStatus.CheckedIn)
        {
            visit.Status = RequestStatus.Completed;
            visit.CheckOutTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"{visit.FullName} checked out.";
        }
        return RedirectToAction(nameof(Dashboard));
    }
}