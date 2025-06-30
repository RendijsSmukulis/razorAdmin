using Microsoft.AspNetCore.Mvc;
using RazorAdmin.Models;
using RazorAdmin.Services;
using RazorAdmin.ViewModels;

namespace RazorAdmin.Controllers;

public class FeaturesController : Controller
{
    private readonly IFeatureService _featureService;

    public FeaturesController(IFeatureService featureService)
    {
        _featureService = featureService;
    }

    public async Task<IActionResult> Index()
    {
        var features = await _featureService.GetAllFeaturesAsync();
        return View(features);
    }

    public async Task<IActionResult> Detail(string featureName)
    {
        var feature = await _featureService.GetFeatureByNameAsync(featureName);
        
        if (feature == null)
        {
            return NotFound();
        }

        var viewModel = new FeatureDetailViewModel
        {
            Feature = feature,
            RelatedFeatures = await _featureService.GetAllFeaturesAsync()
        };

        return View(viewModel);
    }
} 