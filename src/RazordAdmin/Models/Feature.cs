namespace RazorAdmin.Models;

public class Feature
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }
    public int UsageCount { get; set; }
    public int SuccessRate { get; set; }
    public int ErrorCount { get; set; }
    public DateTime? LastUsed { get; set; }
} 