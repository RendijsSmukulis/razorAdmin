using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorAdmin.Pages;

public class FormsModel : PageModel
{
    private readonly ILogger<FormsModel> _logger;

    public FormsModel(ILogger<FormsModel> logger)
    {
        _logger = logger;
    }

    public void OnGet()
    {
    }

    public void OnPost()
    {
        // Handle form submission
    }
} 