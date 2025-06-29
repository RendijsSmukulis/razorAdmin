using System.ComponentModel.DataAnnotations;

namespace RazorAdmin.Models;

/// <summary>
/// Request model for creating a new feature.
/// </summary>
public class CreateFeatureRequest
{
    /// <summary>
    /// The name of the feature. Must be unique.
    /// </summary>
    /// <example>NewFeature</example>
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The description of the feature. Supports Markdown formatting.
    /// </summary>
    /// <example># New Feature\n\nThis is a **new feature** with *Markdown* support.</example>
    [Required]
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The current status of the feature.
    /// </summary>
    /// <example>Active</example>
    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// The category of the feature.
    /// </summary>
    /// <example>Development</example>
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// The icon class for the feature (Material Design Icons).
    /// </summary>
    /// <example>mdi mdi-star mdi-24px text-yellow-500</example>
    [Required]
    [StringLength(200)]
    public string Icon { get; set; } = string.Empty;
}

/// <summary>
/// Request model for updating an existing feature.
/// </summary>
public class UpdateFeatureRequest
{
    /// <summary>
    /// The updated description of the feature. Supports Markdown formatting.
    /// </summary>
    /// <example>Updated description with **new content**.</example>
    public string? Description { get; set; }
    
    /// <summary>
    /// The updated status of the feature.
    /// </summary>
    /// <example>Pending</example>
    public string? Status { get; set; }
    
    /// <summary>
    /// The updated category of the feature.
    /// </summary>
    /// <example>Security</example>
    public string? Category { get; set; }
    
    /// <summary>
    /// The updated icon class for the feature.
    /// </summary>
    /// <example>mdi mdi-shield mdi-24px text-blue-500</example>
    public string? Icon { get; set; }
}

/// <summary>
/// Response model for feature data.
/// </summary>
public class FeatureResponse
{
    /// <summary>
    /// The unique identifier of the feature.
    /// </summary>
    /// <example>1</example>
    public int Id { get; set; }
    
    /// <summary>
    /// The name of the feature.
    /// </summary>
    /// <example>AdBlock</example>
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// The description of the feature.
    /// </summary>
    /// <example># Ad Block Feature\n\nBlocks unwanted advertisements...</example>
    public string Description { get; set; } = string.Empty;
    
    /// <summary>
    /// The current status of the feature.
    /// </summary>
    /// <example>Active</example>
    public string Status { get; set; } = string.Empty;
    
    /// <summary>
    /// The category of the feature.
    /// </summary>
    /// <example>Security</example>
    public string Category { get; set; } = string.Empty;
    
    /// <summary>
    /// The icon class for the feature.
    /// </summary>
    /// <example>mdi mdi-shield-check mdi-24px text-blue-500</example>
    public string Icon { get; set; } = string.Empty;
    
    /// <summary>
    /// The timestamp when the feature was last updated.
    /// </summary>
    /// <example>2025-01-15T10:30:00Z</example>
    public DateTime LastUpdated { get; set; }
    
    /// <summary>
    /// The total number of times the feature has been used.
    /// </summary>
    /// <example>15420</example>
    public int UsageCount { get; set; }
    
    /// <summary>
    /// The success rate percentage of the feature.
    /// </summary>
    /// <example>98</example>
    public int SuccessRate { get; set; }
    
    /// <summary>
    /// The total number of errors encountered by the feature.
    /// </summary>
    /// <example>12</example>
    public int ErrorCount { get; set; }
    
    /// <summary>
    /// The timestamp when the feature was last used.
    /// </summary>
    /// <example>2025-01-15T08:30:00Z</example>
    public DateTime? LastUsed { get; set; }
}

/// <summary>
/// Generic API response wrapper for consistent response formatting.
/// </summary>
/// <typeparam name="T">The type of data being returned.</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    /// <example>true</example>
    public bool Success { get; set; }
    
    /// <summary>
    /// A message describing the result of the operation.
    /// </summary>
    /// <example>Features retrieved successfully</example>
    public string? Message { get; set; }
    
    /// <summary>
    /// The actual data being returned.
    /// </summary>
    public T? Data { get; set; }
    
    /// <summary>
    /// List of validation or error messages if the operation failed.
    /// </summary>
    /// <example>["The Name field is required.", "The Status field is required."]</example>
    public List<string>? Errors { get; set; }
} 