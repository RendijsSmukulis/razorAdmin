using RazorAdmin.Models;
using RazorAdmin.Repositories;
using System.ComponentModel.DataAnnotations;

namespace RazorAdmin.Services;

/// <summary>
/// Service implementation for feature business logic operations.
/// </summary>
public class FeatureService : IFeatureService
{
    private readonly IFeatureRepository _featureRepository;
    private readonly ILogger<FeatureService> _logger;

    public FeatureService(IFeatureRepository featureRepository, ILogger<FeatureService> logger)
    {
        _featureRepository = featureRepository;
        _logger = logger;
    }

    public async Task<IEnumerable<Feature>> GetAllFeaturesAsync()
    {
        try
        {
            return await _featureRepository.GetAllAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all features");
            throw;
        }
    }

    public async Task<Feature?> GetFeatureByIdAsync(int id)
    {
        try
        {
            return await _featureRepository.GetByIdAsync(id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving feature with ID {FeatureId}", id);
            throw;
        }
    }

    public async Task<Feature?> GetFeatureByNameAsync(string name)
    {
        try
        {
            return await _featureRepository.GetByNameAsync(name);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving feature with name {FeatureName}", name);
            throw;
        }
    }

    public async Task<Feature> CreateFeatureAsync(CreateFeatureRequest request)
    {
        try
        {
            // Validate request
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(request);
            if (!Validator.TryValidateObject(request, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(v => v.ErrorMessage).Where(e => e != null).ToList()!;
                throw new ValidationException($"Validation failed: {string.Join(", ", errors)}");
            }

            // Check if feature name already exists
            if (await _featureRepository.ExistsAsync(request.Name))
            {
                throw new InvalidOperationException($"A feature with the name '{request.Name}' already exists");
            }

            var feature = request.ToEntity();
            var id = await _featureRepository.CreateAsync(feature);
            
            var createdFeature = await _featureRepository.GetByIdAsync(id);
            if (createdFeature == null)
            {
                throw new InvalidOperationException("Failed to retrieve created feature");
            }

            _logger.LogInformation("Feature '{FeatureName}' created successfully with ID {FeatureId}", 
                request.Name, id);
            
            return createdFeature;
        }
        catch (Exception ex) when (ex is not ValidationException && ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error creating feature '{FeatureName}'", request.Name);
            throw;
        }
    }

    public async Task<Feature> UpdateFeatureAsync(int id, UpdateFeatureRequest request)
    {
        try
        {
            var existingFeature = await _featureRepository.GetByIdAsync(id);
            if (existingFeature == null)
            {
                throw new InvalidOperationException($"Feature with ID {id} not found");
            }

            existingFeature.UpdateFromRequest(request);
            var success = await _featureRepository.UpdateAsync(existingFeature);
            
            if (!success)
            {
                throw new InvalidOperationException($"Failed to update feature with ID {id}");
            }

            var updatedFeature = await _featureRepository.GetByIdAsync(id);
            if (updatedFeature == null)
            {
                throw new InvalidOperationException("Failed to retrieve updated feature");
            }

            _logger.LogInformation("Feature '{FeatureName}' updated successfully", existingFeature.Name);
            
            return updatedFeature;
        }
        catch (Exception ex) when (ex is not InvalidOperationException)
        {
            _logger.LogError(ex, "Error updating feature with ID {FeatureId}", id);
            throw;
        }
    }

    public async Task<bool> DeleteFeatureAsync(int id)
    {
        try
        {
            var feature = await _featureRepository.GetByIdAsync(id);
            if (feature == null)
            {
                return false;
            }

            var success = await _featureRepository.DeleteAsync(id);
            if (success)
            {
                _logger.LogInformation("Feature '{FeatureName}' deleted successfully", feature.Name);
            }
            
            return success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting feature with ID {FeatureId}", id);
            throw;
        }
    }

    public async Task<bool> FeatureExistsAsync(string name, int? excludeId = null)
    {
        try
        {
            return await _featureRepository.ExistsAsync(name, excludeId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if feature '{FeatureName}' exists", name);
            throw;
        }
    }
} 