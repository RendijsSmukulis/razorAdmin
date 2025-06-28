using Microsoft.AspNetCore.Mvc;

namespace RazorAdmin.Controllers;

public class FormsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
} 