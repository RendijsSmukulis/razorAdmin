using Microsoft.AspNetCore.Mvc;
using RazorAdmin.Models;
using RazorAdmin.Services;
using System.ComponentModel.DataAnnotations;

namespace RazorAdmin.Controllers;

/// <summary>
/// API controller for managing features in the RazorAdmin system.
/// Provides CRUD operations for features with validation and error handling.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class FeaturesApiController : ControllerBase
{
    private readonly IFeatureService _featureService;
    private readonly ILogger<FeaturesApiController> _logger;

    public FeaturesApiController(IFeatureService featureService, ILogger<FeaturesApiController> logger)
    {
        _featureService = featureService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all features from the system.
    /// </summary>
    /// <returns>A list of all features with their details.</returns>
    /// <response code="200">Returns the list of features.</response>
    /// <response code="500">If there was an internal server error.</response>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<FeatureResponse>>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponse<IEnumerable<FeatureResponse>>>> GetAll()
    {
        try
        {
            var features = await _featureService.GetAllFeaturesAsync();
            var responses = features.Select(f => f.ToResponse());
            return Ok(new ApiResponse<IEnumerable<FeatureResponse>>
            {
                Success = true,
                Data = responses,
                Message = "Features retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all features");
            return StatusCode(500, new ApiResponse<IEnumerable<FeatureResponse>>
            {
                Success = false,
                Message = "An error occurred while retrieving features"
            });
        }
    }

    /// <summary>
    /// Retrieves a specific feature by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the feature.</param>
    /// <returns>The feature details if found.</returns>
    /// <response code="200">Returns the requested feature.</response>
    /// <response code="404">If the feature with the specified ID was not found.</response>
    /// <response code="500">If there was an internal server error.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<FeatureResponse>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponse<FeatureResponse>>> GetById(int id)
    {
        try
        {
            var feature = await _featureService.GetFeatureByIdAsync(id);
            if (feature == null)
            {
                return NotFound(new ApiResponse<FeatureResponse>
                {
                    Success = false,
                    Message = "Feature not found"
                });
            }
            return Ok(new ApiResponse<FeatureResponse>
            {
                Success = true,
                Data = feature.ToResponse(),
                Message = "Feature retrieved successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving feature with ID {FeatureId}", id);
            return StatusCode(500, new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "An error occurred while retrieving the feature"
            });
        }
    }

    /// <summary>
    /// Creates a new feature in the system.
    /// </summary>
    /// <param name="request">The feature creation request containing all required fields.</param>
    /// <returns>The newly created feature with its assigned ID.</returns>
    /// <response code="201">Returns the newly created feature.</response>
    /// <response code="400">If the request data is invalid or feature name already exists.</response>
    /// <response code="500">If there was an internal server error.</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<FeatureResponse>), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponse<FeatureResponse>>> Create([FromBody] CreateFeatureRequest request)
    {
        try
        {
            var feature = await _featureService.CreateFeatureAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = feature.Id }, new ApiResponse<FeatureResponse>
            {
                Success = true,
                Data = feature.ToResponse(),
                Message = "Feature created successfully"
            });
        }
        catch (ValidationException ex)
        {
            return BadRequest(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "Validation failed",
                Errors = new List<string> { ex.Message }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating feature '{FeatureName}'", request.Name);
            return StatusCode(500, new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "An error occurred while creating the feature"
            });
        }
    }

    /// <summary>
    /// Updates an existing feature in the system.
    /// </summary>
    /// <param name="id">The unique identifier of the feature to update.</param>
    /// <param name="request">The update request containing the fields to modify.</param>
    /// <returns>The updated feature details.</returns>
    /// <response code="200">Returns the updated feature.</response>
    /// <response code="404">If the feature with the specified ID was not found.</response>
    /// <response code="500">If there was an internal server error.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<FeatureResponse>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponse<FeatureResponse>>> Update(int id, [FromBody] UpdateFeatureRequest request)
    {
        try
        {
            var feature = await _featureService.UpdateFeatureAsync(id, request);
            return Ok(new ApiResponse<FeatureResponse>
            {
                Success = true,
                Data = feature.ToResponse(),
                Message = "Feature updated successfully"
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating feature with ID {FeatureId}", id);
            return StatusCode(500, new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "An error occurred while updating the feature"
            });
        }
    }

    /// <summary>
    /// Deletes a feature from the system.
    /// </summary>
    /// <param name="id">The unique identifier of the feature to delete.</param>
    /// <returns>A success message confirming the deletion.</returns>
    /// <response code="200">Returns a success message.</response>
    /// <response code="404">If the feature with the specified ID was not found.</response>
    /// <response code="500">If there was an internal server error.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(ApiResponse<object>), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        try
        {
            var success = await _featureService.DeleteFeatureAsync(id);
            if (!success)
            {
                return NotFound(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Feature not found"
                });
            }
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Feature deleted successfully"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting feature with ID {FeatureId}", id);
            return StatusCode(500, new ApiResponse<object>
            {
                Success = false,
                Message = "An error occurred while deleting the feature"
            });
        }
    }

    /// <summary>
    /// Health check endpoint to verify the API is running.
    /// </summary>
    /// <returns>Health status and current timestamp.</returns>
    /// <response code="200">Returns health status.</response>
    [HttpGet("/api/health")]
    [ProducesResponseType(200)]
    public IActionResult Health()
    {
        return Ok(new
        {
            Status = "Healthy",
            Timestamp = DateTime.UtcNow,
            Version = "1.0.0"
        });
    }
} 