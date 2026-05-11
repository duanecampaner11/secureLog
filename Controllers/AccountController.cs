[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Register(RegisterViewModel model, string role = "Client")
{
    if (!ModelState.IsValid) return View(model);
    
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
        // Assign role (default is Client)
        await _userManager.AddToRoleAsync(user, role);
        
        await _signInManager.SignInAsync(user, isPersistent: false);
        
        if (role == "Admin")
            return RedirectToAction("Dashboard", "Admin");
        else if (role == "Guard")
            return RedirectToAction("Dashboard", "Guard");
        else
            return RedirectToAction("Dashboard", "Client");
    }
    
    foreach (var err in result.Errors)
        ModelState.AddModelError(string.Empty, err.Description);
    
    return View(model);
}