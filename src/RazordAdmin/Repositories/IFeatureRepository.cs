using RazorAdmin.Models;

namespace RazorAdmin.Repositories;

/// <summary>
/// Repository interface for feature data access operations.
/// </summary>
public interface IFeatureRepository
{
    Task<IEnumerable<Feature>> GetAllAsync();
    Task<Feature?> GetByIdAsync(int id);
    Task<Feature?> GetByNameAsync(string name);
    Task<int> CreateAsync(Feature feature);
    Task<bool> UpdateAsync(Feature feature);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(string name, int? excludeId = null);
} 