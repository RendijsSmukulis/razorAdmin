using Microsoft.AspNetCore.Mvc;
using RazorAdmin.Models;
using RazorAdmin.Services;
using System.ComponentModel.DataAnnotations;

namespace RazorAdmin.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeaturesApiController : ControllerBase
{
    private readonly IDatabaseService _db;

    public FeaturesApiController(IDatabaseService db)
    {
        _db = db;
    }

    [HttpGet]
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

    [HttpGet("{id:int}")]
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

    [HttpPost]
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

    [HttpPut("{id:int}")]
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

    [HttpDelete("{id:int}")]
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

    [HttpGet("/api/health")]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy", Timestamp = DateTime.UtcNow });
    }
} 