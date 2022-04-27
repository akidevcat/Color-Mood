using System.Text.Json.Nodes;
using Color_Mood.Models;

namespace Color_Mood.Services;

public class PexelsSearchService : ISearchService
{
    // HttpClient is thread-safe for specific calls
    private readonly HttpClient _client = new();
    private readonly IPaletteGeneratorService _paletteGeneratorService;
    private readonly ILogger<PexelsSearchService> _logger;

    public PexelsSearchService(IPaletteGeneratorService paletteGeneratorService, IConfiguration configuration, ILogger<PexelsSearchService> logger)
    {
        _client.DefaultRequestHeaders.Add("Authorization", configuration.GetSection("Pexels:Token").Value);
        _paletteGeneratorService = paletteGeneratorService;
        _logger = logger;
    }

    public async Task<List<PaletteResultViewModel>> SearchAsync(string query, int count)
    {
        var result = new List<PaletteResultViewModel>();
        try
        {
            var response = await _client.GetAsync($"https://api.pexels.com/v1/search?query={query}&per_page={count}");
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                dynamic? jsonObject = JsonNode.Parse(jsonString);
                var photos = jsonObject?["photos"];
                foreach (var photo in photos!)
                {
                    var fullUrl = (string?)photo["url"];
                    var photographer = (string?) photo["photographer"];
                    var photographerUrl = (string?) photo["photographer_url"];
                    var url = (string?)photo["src"]["small"];
                    var palette = await _paletteGeneratorService.GeneratePalette(url!, 6);

                    result.Add(new PaletteResultViewModel
                    {
                        Palette = palette,
                        PhotoUrl = fullUrl,
                        Photographer = photographer,
                        PhotographerUrl = photographerUrl
                    });
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to send/parse a Pexels search query");
        }

        return result;
    }
}