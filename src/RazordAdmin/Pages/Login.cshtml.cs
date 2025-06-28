using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorAdmin.Pages;

public class LoginModel : PageModel
{
    private readonly ILogger<LoginModel> _logger;

    public LoginModel(ILogger<LoginModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public void OnPost()
    {
        // Handle login logic
    }
} 