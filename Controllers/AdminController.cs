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
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        // Get pending visit requests
        var pendingRequests = await _db.VisitRequests
            .Include(r => r.ClientUser)
            .Where(r => r.Status == RequestStatus.Pending)
            .OrderBy(r => r.RequestedAt)
            .ToListAsync();
        
        // Get all users with their roles
        var allUsers = await _userManager.Users.ToListAsync();
        var usersWithRoles = new List<UserRoleViewModel>();
        
        foreach (var user in allUsers)
        {
            var roles = await _userManager.GetRolesAsync(user);
            usersWithRoles.Add(new UserRoleViewModel
            {
                User = user,
                Roles = roles.ToList()
            });
        }
        
        ViewBag.UsersWithRoles = usersWithRoles;
        ViewBag.AllRoles = await _roleManager.Roles.ToListAsync();
        
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
        var confirmationId = $"CONF-{DateTime.Now:yyyyMMdd}-{new Random().Next(1000, 9999)}";
        
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
    public async Task<IActionResult> ChangeUserRole(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Dashboard));
        }

        // Remove all current roles
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        
        // Add new role
        await _userManager.AddToRoleAsync(user, newRole);
        
        TempData["Success"] = $"{user.UserName}'s role has been changed to {newRole}.";
        return RedirectToAction(nameof(Dashboard));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            TempData["Error"] = "User not found.";
            return RedirectToAction(nameof(Dashboard));
        }

        // Don't allow admin to delete themselves
        var currentUser = await _userManager.GetUserAsync(User);
        if (user.Id == currentUser.Id)
        {
            TempData["Error"] = "You cannot delete your own account.";
            return RedirectToAction(nameof(Dashboard));
        }

        await _userManager.DeleteAsync(user);
        TempData["Success"] = $"User {user.UserName} has been deleted.";
        return RedirectToAction(nameof(Dashboard));
    }
}

public class UserRoleViewModel
{
    public ApplicationUser User { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}