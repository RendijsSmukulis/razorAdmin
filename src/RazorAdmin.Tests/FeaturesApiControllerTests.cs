using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RazorAdmin.Configuration;
using RazorAdmin.Controllers;
using RazorAdmin.Models;
using RazorAdmin.Services;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace RazorAdmin.Tests;

public class FeaturesApiControllerTests
{
    private readonly Mock<IFeatureService> _mockFeatureService;
    private readonly Mock<ILogger<FeaturesApiController>> _mockLogger;
    private readonly Mock<IOptions<AppSettings>> _mockAppSettings;
    private readonly FeaturesApiController _controller;
    private readonly AppSettings _appSettings;

    public FeaturesApiControllerTests()
    {
        _mockFeatureService = new Mock<IFeatureService>();
        _mockLogger = new Mock<ILogger<FeaturesApiController>>();
        _mockAppSettings = new Mock<IOptions<AppSettings>>();
        
        _appSettings = new AppSettings
        {
            ApplicationName = "RazorAdmin",
            Version = "1.0.0",
            ConnectionString = "Data Source=test.db"
        };
        
        _mockAppSettings.Setup(x => x.Value).Returns(_appSettings);
        
        _controller = new FeaturesApiController(_mockFeatureService.Object, _mockLogger.Object, _mockAppSettings.Object);
    }

    [Fact]
    public async Task GetAll_ShouldReturnOkResultWithFeatures()
    {
        // Arrange
        var features = new List<Feature>
        {
            new() { Id = 1, Name = "Test1", Description = "Test Description 1", Status = "Active", Category = "Test", Icon = "test-icon" },
            new() { Id = 2, Name = "Test2", Description = "Test Description 2", Status = "Active", Category = "Test", Icon = "test-icon" }
        };

        _mockFeatureService.Setup(s => s.GetAllFeaturesAsync()).ReturnsAsync(features);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<IEnumerable<FeatureResponse>>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().HaveCount(2);
        apiResponse.Message.Should().Be("Features retrieved successfully");
    }

    [Fact]
    public async Task GetAll_WhenExceptionOccurs_ShouldReturnInternalServerError()
    {
        // Arrange
        _mockFeatureService.Setup(s => s.GetAllFeaturesAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        var statusResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusResult.StatusCode.Should().Be(500);
        var apiResponse = statusResult.Value.Should().BeOfType<ApiResponse<IEnumerable<FeatureResponse>>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("An error occurred while retrieving features");
    }

    [Fact]
    public async Task GetById_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        var feature = new Feature { Id = 1, Name = "Test", Description = "Test Description", Status = "Active", Category = "Test", Icon = "test-icon" };
        _mockFeatureService.Setup(s => s.GetFeatureByIdAsync(1)).ReturnsAsync(feature);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<FeatureResponse>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(1);
        apiResponse.Data.Name.Should().Be("Test");
    }

    [Fact]
    public async Task GetById_WithInvalidId_ShouldReturnNotFound()
    {
        // Arrange
        _mockFeatureService.Setup(s => s.GetFeatureByIdAsync(999)).ReturnsAsync((Feature?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<FeatureResponse>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Feature not found");
    }

    [Fact]
    public async Task Create_WithValidRequest_ShouldReturnCreatedResult()
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

        _mockFeatureService.Setup(s => s.CreateFeatureAsync(request)).ReturnsAsync(createdFeature);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(FeaturesApiController.GetById));
        createdResult.RouteValues!["id"].Should().Be(1);
        
        var apiResponse = createdResult.Value.Should().BeOfType<ApiResponse<FeatureResponse>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Name.Should().Be("NewFeature");
    }

    [Fact]
    public async Task Create_WithValidationException_ShouldReturnBadRequest()
    {
        // Arrange
        var request = new CreateFeatureRequest
        {
            Name = "",
            Description = "Test Description",
            Status = "Active",
            Category = "Test",
            Icon = "test-icon"
        };

        _mockFeatureService.Setup(s => s.CreateFeatureAsync(request))
            .ThrowsAsync(new ValidationException("Validation failed"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<FeatureResponse>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Validation failed");
        apiResponse.Errors.Should().Contain("Validation failed");
    }

    [Fact]
    public async Task Create_WithInvalidOperationException_ShouldReturnBadRequest()
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

        _mockFeatureService.Setup(s => s.CreateFeatureAsync(request))
            .ThrowsAsync(new InvalidOperationException("Feature name already exists"));

        // Act
        var result = await _controller.Create(request);

        // Assert
        var badRequestResult = result.Result.Should().BeOfType<BadRequestObjectResult>().Subject;
        var apiResponse = badRequestResult.Value.Should().BeOfType<ApiResponse<FeatureResponse>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Feature name already exists");
    }

    [Fact]
    public async Task Update_WithValidRequest_ShouldReturnOkResult()
    {
        // Arrange
        var updateRequest = new UpdateFeatureRequest
        {
            Description = "Updated Description",
            Status = "Pending"
        };

        var updatedFeature = new Feature
        {
            Id = 1,
            Name = "TestFeature",
            Description = "Updated Description",
            Status = "Pending",
            Category = "Test",
            Icon = "test-icon",
            LastUpdated = DateTime.Now
        };

        _mockFeatureService.Setup(s => s.UpdateFeatureAsync(1, updateRequest)).ReturnsAsync(updatedFeature);

        // Act
        var result = await _controller.Update(1, updateRequest);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<FeatureResponse>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Description.Should().Be("Updated Description");
        apiResponse.Data.Status.Should().Be("Pending");
    }

    [Fact]
    public async Task Update_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        var updateRequest = new UpdateFeatureRequest
        {
            Description = "Updated Description"
        };

        _mockFeatureService.Setup(s => s.UpdateFeatureAsync(999, updateRequest))
            .ThrowsAsync(new InvalidOperationException("Feature with ID 999 not found"));

        // Act
        var result = await _controller.Update(999, updateRequest);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<FeatureResponse>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Feature with ID 999 not found");
    }

    [Fact]
    public async Task Delete_WithValidId_ShouldReturnOkResult()
    {
        // Arrange
        _mockFeatureService.Setup(s => s.DeleteFeatureAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var apiResponse = okResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Success.Should().BeTrue();
        apiResponse.Message.Should().Be("Feature deleted successfully");
    }

    [Fact]
    public async Task Delete_WithNonExistentId_ShouldReturnNotFound()
    {
        // Arrange
        _mockFeatureService.Setup(s => s.DeleteFeatureAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        var notFoundResult = result.Result.Should().BeOfType<NotFoundObjectResult>().Subject;
        var apiResponse = notFoundResult.Value.Should().BeOfType<ApiResponse<object>>().Subject;
        apiResponse.Success.Should().BeFalse();
        apiResponse.Message.Should().Be("Feature not found");
    }
} 