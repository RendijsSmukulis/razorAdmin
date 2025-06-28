using RazorAdmin.Models;

namespace RazorAdmin.ViewModels;

public class FeatureDetailViewModel
{
    public Feature Feature { get; set; } = new();
    public IEnumerable<Feature> RelatedFeatures { get; set; } = Enumerable.Empty<Feature>();
} 