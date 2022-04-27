using Color_Mood.Models;

namespace Color_Mood.Services;

public interface IPaletteGeneratorService
{
    Task<Palette> GeneratePalette(string url, int maxPaletteLength);
}