using RazorAdmin.Models;

namespace RazorAdmin.Tests;

/// <summary>
/// Helper class providing test data for unit tests.
/// </summary>
public static class TestData
{
    public static Feature CreateTestFeature(int id = 1, string name = "TestFeature")
    {
        return new Feature
        {
            Id = id,
            Name = name,
            Description = "Test Description",
            Status = "Active",
            Category = "Test",
            Icon = "mdi mdi-test mdi-24px text-blue-500",
            LastUpdated = DateTime.Now,
            UsageCount = 100,
            SuccessRate = 95,
            ErrorCount = 5,
            LastUsed = DateTime.Now.AddHours(-1)
        };
    }

    public static List<Feature> CreateTestFeatures(int count = 3)
    {
        var features = new List<Feature>();
        for (int i = 1; i <= count; i++)
        {
            features.Add(CreateTestFeature(i, $"TestFeature{i}"));
        }
        return features;
    }

    public static CreateFeatureRequest CreateValidCreateRequest()
    {
        return new CreateFeatureRequest
        {
            Name = "NewFeature",
            Description = "New Feature Description",
            Status = "Active",
            Category = "Test",
            Icon = "mdi mdi-new mdi-24px text-green-500"
        };
    }

    public static CreateFeatureRequest CreateInvalidCreateRequest()
    {
        return new CreateFeatureRequest
        {
            Name = "", // Invalid - empty name
            Description = "Test Description",
            Status = "Active",
            Category = "Test",
            Icon = "test-icon"
        };
    }

    public static UpdateFeatureRequest CreateValidUpdateRequest()
    {
        return new UpdateFeatureRequest
        {
            Description = "Updated Description",
            Status = "Pending",
            Category = "Updated",
            Icon = "mdi mdi-updated mdi-24px text-orange-500"
        };
    }

    public static UpdateFeatureRequest CreatePartialUpdateRequest()
    {
        return new UpdateFeatureRequest
        {
            Description = "Updated Description",
            Status = null, // Null - should not update
            Category = "Updated",
            Icon = null // Null - should not update
        };
    }

    public static string CreateTestMarkdown()
    {
        return @"# Test Feature

## Description

This is a **test feature** with *italic* text and `inline code`.

### Features

- Feature 1
- Feature 2
- Feature 3

### Code Example

```csharp
public class TestFeature
{
    public string Name { get; set; }
    public bool IsActive { get; set; }
}
```

### Important Note

> This is an important note about the test feature.

Visit [Documentation](https://example.com) for more information.";
    }
} 