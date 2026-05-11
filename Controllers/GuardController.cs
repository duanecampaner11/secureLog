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

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var today = DateTime.Today;
        var confirmedVisits = await _db.VisitRequests
            .Include(r => r.ClientUser)
            .Where(r => (r.Status == RequestStatus.Confirmed || r.Status == RequestStatus.CheckedIn) 
                        && r.VisitDate.Date == today)
            .OrderBy(r => r.VisitTime)
            .ToListAsync();
            
        return View(confirmedVisits);
    }

    [HttpGet]
    public IActionResult VerifyVisitor()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyVisitor(string confirmationId)
    {
        if (string.IsNullOrWhiteSpace(confirmationId))
        {
            ViewBag.Error = "Please enter a confirmation ID.";
            return View();
        }

        var visit = await _db.VisitRequests
            .Include(r => r.ClientUser)
            .FirstOrDefaultAsync(r => r.ConfirmationId == confirmationId.ToUpper());

        if (visit == null)
        {
            ViewBag.Error = "Invalid confirmation ID.";
            ViewBag.IsValid = false;
            return View();
        }

        if (visit.Status != RequestStatus.Confirmed && visit.Status != RequestStatus.CheckedIn)
        {
            ViewBag.Error = $"This request is {visit.Status}. Access denied.";
            ViewBag.IsValid = false;
            ViewBag.Visit = visit;
            return View();
        }

        if (visit.VisitDate.Date != DateTime.Today)
        {
            ViewBag.Error = $"This confirmation is valid only for {visit.VisitDate:yyyy-MM-dd}. Access denied.";
            ViewBag.IsValid = false;
            ViewBag.Visit = visit;
            return View();
        }

        ViewBag.IsValid = true;
        ViewBag.Visit = visit;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckIn(int id)
    {
        var request = await _db.VisitRequests.FindAsync(id);
        if (request != null && request.Status == RequestStatus.Confirmed)
        {
            request.Status = RequestStatus.CheckedIn;
            request.CheckInTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            
            // Also add to legacy GuestEntries
            var entry = new GuestEntry
            {
                Name = request.FullName,
                Purpose = request.Purpose,
                TimeIn = DateTime.UtcNow,
                LoggedByUserId = "Guard",
                LoggedAt = DateTime.UtcNow
            };
            _db.GuestEntries.Add(entry);
            await _db.SaveChangesAsync();
            
            TempData["Success"] = $"{request.FullName} checked in successfully.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckOut(int id)
    {
        var request = await _db.VisitRequests.FindAsync(id);
        if (request != null && request.Status == RequestStatus.CheckedIn)
        {
            request.Status = RequestStatus.Completed;
            request.CheckOutTime = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            TempData["Success"] = $"{request.FullName} checked out.";
        }
        return RedirectToAction(nameof(Dashboard));
    }
}