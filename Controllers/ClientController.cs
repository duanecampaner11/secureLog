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
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var requests = await _db.VisitRequests
            .Where(r => r.ClientUserId == user.Id)
            .OrderByDescending(r => r.RequestedAt)
            .ToListAsync();
            
        return View(requests);
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
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return RedirectToAction("Login", "Account");
        }
        
        var request = new VisitRequest
        {
            ClientUserId = user.Id,
            FullName = model.FullName,
            Company = model.Company,
            Purpose = model.Purpose,
            PersonToMeet = model.PersonToMeet,
            VisitDate = model.VisitDate.Date,
            VisitTime = model.VisitTime,
            Notes = model.Notes,
            Status = RequestStatus.Pending,
            RequestedAt = DateTime.UtcNow
        };

        _db.VisitRequests.Add(request);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Your visit request has been submitted. Please wait for admin confirmation.";
        return RedirectToAction(nameof(Dashboard));
    }

   [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> CreateRequest(VisitRequest model)
{
    if (!ModelState.IsValid)
    {
        TempData["Error"] = "Please fill in all required fields.";
        return View(model);
    }
    
    var user = await _userManager.GetUserAsync(User);
    if (user == null)
    {
        return RedirectToAction("Login", "Account");
    }
    
    try
    {
        var request = new VisitRequest
        {
            ClientUserId = user.Id,
            FullName = model.FullName,
            Company = model.Company,
            Purpose = model.Purpose,
            PersonToMeet = model.PersonToMeet,
            VisitDate = model.VisitDate,
            VisitTime = model.VisitTime,
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
        return View(model);
    }
}}