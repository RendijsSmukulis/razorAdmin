using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorAdmin.Pages;

public class ProfileModel : PageModel
{
    private readonly ILogger<ProfileModel> _logger;

    public ProfileModel(ILogger<ProfileModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
} 