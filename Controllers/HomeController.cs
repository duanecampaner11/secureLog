using Microsoft.AspNetCore.Mvc;

namespace SecureLog.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Guest");
        return RedirectToAction("Login", "Account");
    }

    public IActionResult Error() => View();
}
