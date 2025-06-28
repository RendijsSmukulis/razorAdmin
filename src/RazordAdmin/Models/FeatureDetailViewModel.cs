namespace RazorAdmin.Models;

public class FeatureDetailViewModel
{
    public string FeatureName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string StatusColor { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string LastUpdated { get; set; } = string.Empty;
    public string FeatureIcon { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public int SuccessRate { get; set; }
    public int ErrorCount { get; set; }
    public string LastUsed { get; set; } = string.Empty;
} 