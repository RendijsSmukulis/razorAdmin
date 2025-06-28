using Microsoft.AspNetCore.Mvc;
using RazorAdmin.Models;

namespace RazorAdmin.Controllers;

public class FeaturesController : Controller
{
    private readonly ILogger<FeaturesController> _logger;

    public FeaturesController(ILogger<FeaturesController> logger)
    {
        _logger = logger;
    }

    // GET: /Features
    public IActionResult Index()
    {
        return View();
    }

    // GET: /Features/{featureName}
    public IActionResult Detail(string featureName)
    {
        // Sample feature data - in a real application, this would come from a database
        var featureData = GetFeatureData(featureName);
        
        var model = new FeatureDetailViewModel
        {
            FeatureName = featureName,
            Description = featureData.Description,
            Status = featureData.Status,
            StatusColor = GetStatusColor(featureData.Status),
            Category = featureData.Category,
            LastUpdated = featureData.LastUpdated,
            FeatureIcon = featureData.Icon,
            UsageCount = featureData.UsageCount,
            SuccessRate = featureData.SuccessRate,
            ErrorCount = featureData.ErrorCount,
            LastUsed = featureData.LastUsed
        };

        return View(model);
    }

    // POST: /Features/{featureName}
    [HttpPost]
    public IActionResult Detail(string featureName, FeatureDetailViewModel model)
    {
        // Handle form submission - in a real application, this would save to a database
        _logger.LogInformation("Saving changes for feature: {FeatureName}", featureName);
        
        // Redirect back to the feature detail page
        return RedirectToAction(nameof(Detail), new { featureName });
    }

    private static (string Description, string Status, string Category, string LastUpdated, string Icon, int UsageCount, int SuccessRate, int ErrorCount, string LastUsed) GetFeatureData(string featureName)
    {
        return featureName.ToLower() switch
        {
            "adblock" => ("Block unwanted advertisements and tracking scripts", "Active", "Security", "Jan 15, 2025", "mdi mdi-shield-check mdi-24px text-blue-500", 15420, 98, 12, "2 hours ago"),
            "devtools" => ("Developer tools and debugging utilities", "Active", "Development", "Jan 12, 2025", "mdi mdi-tools mdi-24px text-green-500", 8920, 95, 45, "1 hour ago"),
            "database" => ("Database administration and monitoring tools", "Active", "Data", "Jan 10, 2025", "mdi mdi-database mdi-24px text-purple-500", 5670, 99, 3, "30 minutes ago"),
            "usermanagement" => ("User accounts, roles, and permissions management", "Active", "Administration", "Jan 8, 2025", "mdi mdi-account-group mdi-24px text-orange-500", 12340, 97, 23, "15 minutes ago"),
            "analytics" => ("System analytics and reporting dashboard", "Pending", "Reporting", "Jan 5, 2025", "mdi mdi-chart-line mdi-24px text-red-500", 0, 0, 0, "Never"),
            "backup" => ("System backup and data recovery tools", "Active", "System", "Jan 3, 2025", "mdi mdi-backup-restore mdi-24px text-indigo-500", 2340, 100, 0, "1 day ago"),
            "notifications" => ("System notification and alert management", "Disabled", "Communication", "Dec 28, 2024", "mdi mdi-bell mdi-24px text-pink-500", 0, 0, 0, "Never"),
            _ => ("Unknown feature", "Disabled", "Unknown", "Unknown", "mdi mdi-help-circle mdi-24px text-gray-500", 0, 0, 0, "Never")
        };
    }

    private static string GetStatusColor(string status)
    {
        return status switch
        {
            "Active" => "green",
            "Pending" => "yellow",
            "Disabled" => "red",
            _ => "gray"
        };
    }
} 