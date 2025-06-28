using Microsoft.AspNetCore.Mvc;

namespace RazorAdmin.Controllers;

public class ProfileController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
} 