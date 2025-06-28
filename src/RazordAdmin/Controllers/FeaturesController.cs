using Microsoft.AspNetCore.Mvc;
using RazorAdmin.Models;
using RazorAdmin.Services;
using RazorAdmin.ViewModels;

namespace RazorAdmin.Controllers;

public class FeaturesController : Controller
{
    private readonly IDatabaseService _databaseService;

    public FeaturesController(IDatabaseService databaseService)
    {
        _databaseService = databaseService;
    }

    public async Task<IActionResult> Index()
    {
        var features = await _databaseService.GetAllFeaturesAsync();
        return View(features);
    }

    public async Task<IActionResult> Detail(string featureName)
    {
        var feature = await _databaseService.GetFeatureByNameAsync(featureName);
        
        if (feature == null)
        {
            return NotFound();
        }

        var viewModel = new FeatureDetailViewModel
        {
            Feature = feature,
            RelatedFeatures = await _databaseService.GetAllFeaturesAsync()
        };

        return View(viewModel);
    }
} 