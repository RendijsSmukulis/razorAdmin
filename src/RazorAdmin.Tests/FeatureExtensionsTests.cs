using FluentAssertions;
using RazorAdmin.Models;
using Xunit;

namespace RazorAdmin.Tests;

public class FeatureExtensionsTests
{
    [Fact]
    public void ToResponse_ShouldMapFeatureToFeatureResponse()
    {
        // Arrange
        var feature = new Feature
        {
            Id = 1,
            Name = "TestFeature",
            Description = "Test Description",
            Status = "Active",
            Category = "Test",
            Icon = "test-icon",
            LastUpdated = DateTime.Now,
            UsageCount = 100,
            SuccessRate = 95,
            ErrorCount = 5,
            LastUsed = DateTime.Now.AddHours(-1)
        };

        // Act
        var result = feature.ToResponse();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(feature.Id);
        result.Name.Should().Be(feature.Name);
        result.Description.Should().Be(feature.Description);
        result.Status.Should().Be(feature.Status);
        result.Category.Should().Be(feature.Category);
        result.Icon.Should().Be(feature.Icon);
        result.LastUpdated.Should().Be(feature.LastUpdated);
        result.UsageCount.Should().Be(feature.UsageCount);
        result.SuccessRate.Should().Be(feature.SuccessRate);
        result.ErrorCount.Should().Be(feature.ErrorCount);
        result.LastUsed.Should().Be(feature.LastUsed);
    }

    [Fact]
    public void ToEntity_ShouldMapCreateFeatureRequestToFeature()
    {
        // Arrange
        var request = new CreateFeatureRequest
        {
            Name = "NewFeature",
            Description = "New Feature Description",
            Status = "Active",
            Category = "Test",
            Icon = "new-icon"
        };

        // Act
        var result = request.ToEntity();

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(0); // Default value
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        result.Status.Should().Be(request.Status);
        result.Category.Should().Be(request.Category);
        result.Icon.Should().Be(request.Icon);
        result.LastUpdated.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        result.UsageCount.Should().Be(0);
        result.SuccessRate.Should().Be(0);
        result.ErrorCount.Should().Be(0);
        result.LastUsed.Should().BeNull();
    }

    [Fact]
    public void UpdateFromRequest_ShouldUpdateFeatureWithNonNullValues()
    {
        // Arrange
        var feature = new Feature
        {
            Id = 1,
            Name = "OriginalFeature",
            Description = "Original Description",
            Status = "Active",
            Category = "Original",
            Icon = "original-icon",
            LastUpdated = DateTime.Now.AddDays(-1),
            UsageCount = 100,
            SuccessRate = 95,
            ErrorCount = 5,
            LastUsed = DateTime.Now.AddHours(-1)
        };

        var updateRequest = new UpdateFeatureRequest
        {
            Description = "Updated Description",
            Status = "Pending",
            Category = "Updated",
            Icon = "updated-icon"
        };

        var originalLastUpdated = feature.LastUpdated;

        // Act
        feature.UpdateFromRequest(updateRequest);

        // Assert
        feature.Id.Should().Be(1); // Should not change
        feature.Name.Should().Be("OriginalFeature"); // Should not change
        feature.Description.Should().Be("Updated Description");
        feature.Status.Should().Be("Pending");
        feature.Category.Should().Be("Updated");
        feature.Icon.Should().Be("updated-icon");
        feature.LastUpdated.Should().BeAfter(originalLastUpdated);
        feature.UsageCount.Should().Be(100); // Should not change
        feature.SuccessRate.Should().Be(95); // Should not change
        feature.ErrorCount.Should().Be(5); // Should not change
        feature.LastUsed.Should().Be(DateTime.Now.AddHours(-1)); // Should not change
    }

    [Fact]
    public void UpdateFromRequest_WithNullValues_ShouldNotUpdateFields()
    {
        // Arrange
        var feature = new Feature
        {
            Id = 1,
            Name = "OriginalFeature",
            Description = "Original Description",
            Status = "Active",
            Category = "Original",
            Icon = "original-icon",
            LastUpdated = DateTime.Now.AddDays(-1),
            UsageCount = 100,
            SuccessRate = 95,
            ErrorCount = 5,
            LastUsed = DateTime.Now.AddHours(-1)
        };

        var updateRequest = new UpdateFeatureRequest
        {
            Description = null,
            Status = null,
            Category = null,
            Icon = null
        };

        var originalValues = new
        {
            Description = feature.Description,
            Status = feature.Status,
            Category = feature.Category,
            Icon = feature.Icon,
            LastUpdated = feature.LastUpdated
        };

        // Act
        feature.UpdateFromRequest(updateRequest);

        // Assert
        feature.Description.Should().Be(originalValues.Description);
        feature.Status.Should().Be(originalValues.Status);
        feature.Category.Should().Be(originalValues.Category);
        feature.Icon.Should().Be(originalValues.Icon);
        feature.LastUpdated.Should().BeAfter(originalValues.LastUpdated); // Only LastUpdated should change
    }

    [Fact]
    public void UpdateFromRequest_WithPartialNullValues_ShouldUpdateOnlyNonNullFields()
    {
        // Arrange
        var feature = new Feature
        {
            Id = 1,
            Name = "OriginalFeature",
            Description = "Original Description",
            Status = "Active",
            Category = "Original",
            Icon = "original-icon",
            LastUpdated = DateTime.Now.AddDays(-1),
            UsageCount = 100,
            SuccessRate = 95,
            ErrorCount = 5,
            LastUsed = DateTime.Now.AddHours(-1)
        };

        var updateRequest = new UpdateFeatureRequest
        {
            Description = "Updated Description",
            Status = null, // Null - should not update
            Category = "Updated",
            Icon = null // Null - should not update
        };

        var originalStatus = feature.Status;
        var originalIcon = feature.Icon;

        // Act
        feature.UpdateFromRequest(updateRequest);

        // Assert
        feature.Description.Should().Be("Updated Description");
        feature.Status.Should().Be(originalStatus); // Should not change
        feature.Category.Should().Be("Updated");
        feature.Icon.Should().Be(originalIcon); // Should not change
        feature.LastUpdated.Should().BeAfter(DateTime.Now.AddDays(-1)); // Should be updated
    }
} 