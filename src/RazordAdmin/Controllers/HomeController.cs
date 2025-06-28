using Microsoft.AspNetCore.Mvc;

namespace RazorAdmin.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
} 