using Color_Mood.Models;

namespace Color_Mood.Services;

public interface ISearchService
{
    Task<List<PaletteResultViewModel>> SearchAsync(string query, int count);
}