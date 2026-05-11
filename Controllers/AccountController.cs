using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SecureLog.Models;
using SecureLog.Data;
using Microsoft.EntityFrameworkCore;

namespace SecureLog.Controllers;

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ApplicationDbContext _db;

    public AccountController(UserManager<ApplicationUser> userManager, 
                             SignInManager<ApplicationUser> signInManager,
                             ApplicationDbContext db)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _db = db;
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, string role = "Client")
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var user = new ApplicationUser
        {
            UserName = model.UserName,
            Email = model.Email,
            FullName = model.FullName,
            CompanyName = model.CompanyName,
            PhoneNumber = model.PhoneNumber,
            IsApproved = true,
            CreatedAt = DateTime.UtcNow
        };
        
        var result = await _userManager.CreateAsync(user, model.Password);
        
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, role);
            await _signInManager.SignInAsync(user, isPersistent: false);
            
            if (role == "Admin")
                return RedirectToAction("Dashboard", "Admin");
            else if (role == "Guard")
                return RedirectToAction("Dashboard", "Guard");
            else
                return RedirectToAction("Dashboard", "Client");
        }
        
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }
        
        return View(model);
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }
        
        var user = await _userManager.FindByNameAsync(model.UserName);
        
        if (user != null && !user.IsApproved)
        {
            ModelState.AddModelError(string.Empty, "Your account is pending admin approval.");
            return View(model);
        }
        
        var result = await _signInManager.PasswordSignInAsync(model.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);
        
        if (result.Succeeded)
        {
            if (await _userManager.IsInRoleAsync(user!, "Admin"))
                return RedirectToAction("Dashboard", "Admin");
            else if (await _userManager.IsInRoleAsync(user!, "Guard"))
                return RedirectToAction("Dashboard", "Guard");
            else
                return RedirectToAction("Dashboard", "Client");
        }
        
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }
}