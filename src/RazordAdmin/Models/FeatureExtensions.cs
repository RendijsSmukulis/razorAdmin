namespace RazorAdmin.Models;

public static class FeatureExtensions
{
    public static FeatureResponse ToResponse(this Feature feature)
    {
        return new FeatureResponse
        {
            Id = feature.Id,
            Name = feature.Name,
            Description = feature.Description,
            Status = feature.Status,
            Category = feature.Category,
            Icon = feature.Icon,
            LastUpdated = feature.LastUpdated,
            UsageCount = feature.UsageCount,
            SuccessRate = feature.SuccessRate,
            ErrorCount = feature.ErrorCount,
            LastUsed = feature.LastUsed
        };
    }

    public static Feature ToEntity(this CreateFeatureRequest request)
    {
        return new Feature
        {
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            Category = request.Category,
            Icon = request.Icon,
            LastUpdated = DateTime.UtcNow,
            UsageCount = 0,
            SuccessRate = 0,
            ErrorCount = 0,
            LastUsed = null
        };
    }

    public static void UpdateFromRequest(this Feature feature, UpdateFeatureRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Description))
            feature.Description = request.Description;
        
        if (!string.IsNullOrWhiteSpace(request.Status))
            feature.Status = request.Status;
        
        if (!string.IsNullOrWhiteSpace(request.Category))
            feature.Category = request.Category;
        
        if (!string.IsNullOrWhiteSpace(request.Icon))
            feature.Icon = request.Icon;
        
        feature.LastUpdated = DateTime.UtcNow;
    }
} 