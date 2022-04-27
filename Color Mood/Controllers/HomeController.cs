using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Color_Mood.Models;
using Color_Mood.Services;

namespace Color_Mood.Controllers;

public class HomeController : Controller
{
    private readonly ISearchService _searchService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ISearchService searchService)
    {
        _searchService = searchService;
        _logger = logger;
    }

    [HttpGet("")]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet("search")]
    public async Task<IActionResult> Search([MaxLength(50)] [FromQuery(Name = "q")] string? searchQuery)
    {
        if (searchQuery == null)
        {
            return View("Index");
        }
        
        if (!ModelState.IsValid)
        {
            return Problem(statusCode: 400);
        }
        
        searchQuery = Uri.EscapeDataString(searchQuery);

        var result = new SearchViewModel
        {
            SearchResults = await _searchService.SearchAsync(searchQuery, 15)
        };

        return View("Index", result);
    }
    
    public IActionResult Privacy()
    {
        return View();
    }
    
    [HttpGet("palette")]
    public IActionResult RenderPalette([MaxLength(36)] [FromQuery(Name = "p")] string palette)
    {
        if (!ModelState.IsValid || !Palette.TryParse(palette, out var result, false))
        {
            return Problem("Invalid palette format received", palette, 400);
        }
        
        return View(result);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}