using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureLog.Data;
using SecureLog.Models;

namespace SecureLog.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var pendingRequests = await _db.VisitRequests
            .Include(r => r.ClientUser)
            .Where(r => r.Status == RequestStatus.Pending)
            .OrderBy(r => r.RequestedAt)
            .ToListAsync();
            
        var allUsers = await _userManager.Users.ToListAsync();
        
        ViewBag.AllUsers = allUsers;
        return View(pendingRequests);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmRequest(int id)
    {
        var request = await _db.VisitRequests.FindAsync(id);
        if (request == null)
        {
            TempData["Error"] = "Request not found.";
            return RedirectToAction(nameof(Dashboard));
        }

        var admin = await _userManager.GetUserAsync(User);
        
        // Generate unique confirmation ID
        var confirmationId = GenerateConfirmationId(request);
        
        request.Status = RequestStatus.Confirmed;
        request.ConfirmationId = confirmationId;
        request.ReviewedByUserId = admin.Id;
        request.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Success"] = $"Request confirmed! Confirmation ID: {confirmationId}";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReturnRequest(int id, string returnReason)
    {
        var request = await _db.VisitRequests.FindAsync(id);
        if (request == null)
        {
            TempData["Error"] = "Request not found.";
            return RedirectToAction(nameof(Dashboard));
        }

        var admin = await _userManager.GetUserAsync(User);
        
        request.Status = RequestStatus.Returned;
        request.ReturnReason = returnReason;
        request.ReviewedByUserId = admin.Id;
        request.ReviewedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync();

        TempData["Info"] = "Request has been returned to client.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user != null)
        {
            await _userManager.DeleteAsync(user);
            TempData["Success"] = $"User {user.Email} has been deleted.";
        }
        return RedirectToAction(nameof(Dashboard));
    }

    private string GenerateConfirmationId(VisitRequest request)
    {
        var datePart = request.VisitDate.ToString("yyyyMMdd");
        var randomPart = new Random().Next(1000, 9999).ToString();
        return $"CONF-{datePart}-{randomPart}";
    }
}