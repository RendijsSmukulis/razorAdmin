using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using RazorAdmin.Models;
using RazorAdmin.Repositories;
using RazorAdmin.Services;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace RazorAdmin.Tests;

public class FeatureServiceTests
{
    private readonly Mock<IFeatureRepository> _mockRepository;
    private readonly Mock<ILogger<FeatureService>> _mockLogger;
    private readonly FeatureService _service;

    public FeatureServiceTests()
    {
        _mockRepository = new Mock<IFeatureRepository>();
        _mockLogger = new Mock<ILogger<FeatureService>>();
        _service = new FeatureService(_mockRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAllFeaturesAsync_ShouldReturnAllFeatures()
    {
        // Arrange
        var expectedFeatures = new List<Feature>
        {
            new() { Id = 1, Name = "Test1", Description = "Test Description 1", Status = "Active", Category = "Test", Icon = "test-icon" },
            new() { Id = 2, Name = "Test2", Description = "Test Description 2", Status = "Active", Category = "Test", Icon = "test-icon" }
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(expectedFeatures);

        // Act
        var result = await _service.GetAllFeaturesAsync();

        // Assert
        result.Should().BeEquivalentTo(expectedFeatures);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Fact]
    public async Task GetFeatureByIdAsync_WithValidId_ShouldReturnFeature()
    {
        // Arrange
        var expectedFeature = new Feature { Id = 1, Name = "Test", Description = "Test Description", Status = "Active", Category = "Test", Icon = "test-icon" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expectedFeature);

        // Act
        var result = await _service.GetFeatureByIdAsync(1);

        // Assert
        result.Should().BeEquivalentTo(expectedFeature);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task GetFeatureByIdAsync_WithInvalidId_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Feature?)null);

        // Act
        var result = await _service.GetFeatureByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Fact]
    public async Task CreateFeatureAsync_WithValidRequest_ShouldCreateFeature()
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

        var createdFeature = new Feature
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            Status = request.Status,
            Category = request.Category,
            Icon = request.Icon,
            LastUpdated = DateTime.Now
        };

        _mockRepository.Setup(r => r.ExistsAsync(request.Name, It.IsAny<int?>())).ReturnsAsync(false);
        _mockRepository.Setup(r => r.CreateAsync(It.IsAny<Feature>())).ReturnsAsync(1);
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(createdFeature);

        // Act
        var result = await _service.CreateFeatureAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Name.Should().Be(request.Name);
        result.Description.Should().Be(request.Description);
        _mockRepository.Verify(r => r.ExistsAsync(request.Name, It.IsAny<int?>()), Times.Once);
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Feature>()), Times.Once);
    }

    [Fact]
    public async Task CreateFeatureAsync_WithDuplicateName_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var request = new CreateFeatureRequest
        {
            Name = "ExistingFeature",
            Description = "Test Description",
            Status = "Active",
            Category = "Test",
            Icon = "test-icon"
        };

        _mockRepository.Setup(r => r.ExistsAsync(request.Name, It.IsAny<int?>())).ReturnsAsync(true);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.CreateFeatureAsync(request));
        exception.Message.Should().Contain("already exists");
        _mockRepository.Verify(r => r.CreateAsync(It.IsAny<Feature>()), Times.Never);
    }

    [Fact]
    public async Task CreateFeatureAsync_WithInvalidRequest_ShouldThrowValidationException()
    {
        // Arrange
        var request = new CreateFeatureRequest
        {
            Name = "", // Invalid - empty name
            Description = "Test Description",
            Status = "Active",
            Category = "Test",
            Icon = "test-icon"
        };

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ValidationException>(() => _service.CreateFeatureAsync(request));
        exception.Message.Should().Contain("Validation failed");
    }

    [Fact]
    public async Task UpdateFeatureAsync_WithValidRequest_ShouldUpdateFeature()
    {
        // Arrange
        var existingFeature = new Feature
        {
            Id = 1,
            Name = "ExistingFeature",
            Description = "Old Description",
            Status = "Active",
            Category = "Test",
            Icon = "old-icon",
            LastUpdated = DateTime.Now.AddDays(-1)
        };

        var updateRequest = new UpdateFeatureRequest
        {
            Description = "Updated Description",
            Status = "Pending"
        };

        var updatedFeature = new Feature
        {
            Id = 1,
            Name = "ExistingFeature",
            Description = "Updated Description",
            Status = "Pending",
            Category = "Test",
            Icon = "old-icon",
            LastUpdated = DateTime.Now
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existingFeature);
        _mockRepository.Setup(r => r.UpdateAsync(It.IsAny<Feature>())).ReturnsAsync(true);
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(updatedFeature);

        // Act
        var result = await _service.UpdateFeatureAsync(1, updateRequest);

        // Assert
        result.Should().NotBeNull();
        result.Description.Should().Be("Updated Description");
        result.Status.Should().Be("Pending");
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Exactly(2));
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Feature>()), Times.Once);
    }

    [Fact]
    public async Task UpdateFeatureAsync_WithNonExistentId_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var updateRequest = new UpdateFeatureRequest
        {
            Description = "Updated Description"
        };

        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Feature?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.UpdateFeatureAsync(999, updateRequest));
        exception.Message.Should().Contain("not found");
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Feature>()), Times.Never);
    }

    [Fact]
    public async Task DeleteFeatureAsync_WithValidId_ShouldReturnTrue()
    {
        // Arrange
        var feature = new Feature { Id = 1, Name = "TestFeature" };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(feature);
        _mockRepository.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _service.DeleteFeatureAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteFeatureAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Feature?)null);

        // Act
        var result = await _service.DeleteFeatureAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.DeleteAsync(999), Times.Never);
    }

    [Fact]
    public async Task FeatureExistsAsync_ShouldReturnRepositoryResult()
    {
        // Arrange
        _mockRepository.Setup(r => r.ExistsAsync("TestFeature", It.IsAny<int?>())).ReturnsAsync(true);

        // Act
        var result = await _service.FeatureExistsAsync("TestFeature");

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.ExistsAsync("TestFeature", It.IsAny<int?>()), Times.Once);
    }
} 