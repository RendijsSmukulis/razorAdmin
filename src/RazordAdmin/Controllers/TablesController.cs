using Microsoft.AspNetCore.Mvc;

namespace RazorAdmin.Controllers;

public class TablesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
} 