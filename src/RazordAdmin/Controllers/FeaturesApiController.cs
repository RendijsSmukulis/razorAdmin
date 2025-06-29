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
    private readonly IDatabaseService _db;

    public FeaturesApiController(IDatabaseService db)
    {
        _db = db;
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
        var features = await _db.GetAllFeaturesAsync();
        var responses = features.Select(f => f.ToResponse());
        return Ok(new ApiResponse<IEnumerable<FeatureResponse>>
        {
            Success = true,
            Data = responses,
            Message = "Features retrieved successfully"
        });
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
        var feature = await _db.GetFeatureByIdAsync(id);
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
        // Validate request
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(request);
        if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
        {
            return BadRequest(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "Validation failed",
                Errors = validationResults.Select(v => v.ErrorMessage).Where(e => e != null).ToList()!
            });
        }
        // Check if feature name already exists
        if (await _db.FeatureExistsAsync(request.Name))
        {
            return BadRequest(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "Feature name already exists",
                Errors = new List<string> { "A feature with this name already exists" }
            });
        }
        var feature = request.ToEntity();
        var id = await _db.CreateFeatureAsync(feature);
        var createdFeature = await _db.GetFeatureByIdAsync(id);
        return CreatedAtAction(nameof(GetById), new { id }, new ApiResponse<FeatureResponse>
        {
            Success = true,
            Data = createdFeature!.ToResponse(),
            Message = "Feature created successfully"
        });
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
        var feature = await _db.GetFeatureByIdAsync(id);
        if (feature == null)
        {
            return NotFound(new ApiResponse<FeatureResponse>
            {
                Success = false,
                Message = "Feature not found"
            });
        }
        feature.UpdateFromRequest(request);
        var success = await _db.UpdateFeatureAsync(feature);
        if (!success)
        {
            return Problem("Failed to update feature");
        }
        var updatedFeature = await _db.GetFeatureByIdAsync(id);
        return Ok(new ApiResponse<FeatureResponse>
        {
            Success = true,
            Data = updatedFeature!.ToResponse(),
            Message = "Feature updated successfully"
        });
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
        var feature = await _db.GetFeatureByIdAsync(id);
        if (feature == null)
        {
            return NotFound(new ApiResponse<object>
            {
                Success = false,
                Message = "Feature not found"
            });
        }
        var success = await _db.DeleteFeatureAsync(id);
        if (!success)
        {
            return Problem("Failed to delete feature");
        }
        return Ok(new ApiResponse<object>
        {
            Success = true,
            Message = "Feature deleted successfully"
        });
    }

    /// <summary>
    /// Health check endpoint to verify the API is running.
    /// </summary>
    /// <returns>Health status and current timestamp.</returns>
    /// <response code="200">Returns the health status.</response>
    [HttpGet("/api/health")]
    [ProducesResponseType(200)]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
} 