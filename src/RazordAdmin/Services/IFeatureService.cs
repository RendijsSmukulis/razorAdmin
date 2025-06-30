using RazorAdmin.Models;

namespace RazorAdmin.Services;

/// <summary>
/// Service interface for feature business logic operations.
/// </summary>
public interface IFeatureService
{
    Task<IEnumerable<Feature>> GetAllFeaturesAsync();
    Task<Feature?> GetFeatureByIdAsync(int id);
    Task<Feature?> GetFeatureByNameAsync(string name);
    Task<Feature> CreateFeatureAsync(CreateFeatureRequest request);
    Task<Feature> UpdateFeatureAsync(int id, UpdateFeatureRequest request);
    Task<bool> DeleteFeatureAsync(int id);
    Task<bool> FeatureExistsAsync(string name, int? excludeId = null);
} 