using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorAdmin.Pages;

public class FeaturesModel : PageModel
{
    private readonly ILogger<FeaturesModel> _logger;

    public FeaturesModel(ILogger<FeaturesModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }
} 