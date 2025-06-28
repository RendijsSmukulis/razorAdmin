using Microsoft.AspNetCore.Mvc;

namespace RazorAdmin.Controllers;

public class AccountController : Controller
{
    public IActionResult Login()
    {
        return View();
    }
} 