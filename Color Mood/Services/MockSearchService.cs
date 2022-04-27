using Color_Mood.Models;

namespace Color_Mood.Services;

public class MockSearchService : ISearchService
{
    public Task<List<PaletteResultViewModel>> SearchAsync(string query, int count)
    {
        var result = new List<PaletteResultViewModel>
        {
            new PaletteResultViewModel
            {
                Palette = new Palette(new Color(255)),
                Photographer = "Photographer"
            }
        };

        return Task.FromResult(result);
    }
}