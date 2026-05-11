using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureLog.Data;
using SecureLog.Models;

namespace SecureLog.Controllers;

[Authorize(Roles = "Client")]
public class ClientController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public ClientController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account");
            
            var requests = await _db.VisitRequests
                .Where(r => r.ClientUserId == user.Id)
                .OrderByDescending(r => r.RequestedAt)
                .ToListAsync();
                
            return View(requests);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error loading dashboard: {ex.Message}";
            return View(new List<VisitRequest>());
        }
    }

    [HttpGet]
    public IActionResult CreateRequest()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateRequest(VisitRequest model)
    {
        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }
            
            var request = new VisitRequest
            {
                ClientUserId = user.Id,
                FullName = string.IsNullOrWhiteSpace(model.FullName) ? "Not provided" : model.FullName,
                Company = model.Company,
                Purpose = string.IsNullOrWhiteSpace(model.Purpose) ? "Not specified" : model.Purpose,
                PersonToMeet = string.IsNullOrWhiteSpace(model.PersonToMeet) ? "Not specified" : model.PersonToMeet,
                VisitDate = model.VisitDate == default ? DateTime.Today : model.VisitDate,
                Notes = model.Notes,
                Status = RequestStatus.Pending,
                RequestedAt = DateTime.UtcNow
            };
            
            _db.VisitRequests.Add(request);
            await _db.SaveChangesAsync();
            
            TempData["Success"] = "Your visit request has been submitted successfully!";
            return RedirectToAction(nameof(Dashboard));
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error: {ex.Message}";
            return RedirectToAction(nameof(CreateRequest));
        }
    }
}