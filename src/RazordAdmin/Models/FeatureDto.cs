using System.ComponentModel.DataAnnotations;

namespace RazorAdmin.Models;

public class CreateFeatureRequest
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    [Required]
    public string Description { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
    
    [Required]
    [StringLength(200)]
    public string Icon { get; set; } = string.Empty;
}

public class UpdateFeatureRequest
{
    public string? Description { get; set; }
    public string? Status { get; set; }
    public string? Category { get; set; }
    public string? Icon { get; set; }
}

public class FeatureResponse
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

public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
} 