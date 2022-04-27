using System.Diagnostics;
using Color_Mood.Models;
using SkiaSharp;

namespace Color_Mood.Services;

public class PaletteGeneratorService : IPaletteGeneratorService
{
    private const int SAMPLING_RATE = 4;
    private const int COMPUTE_REGION_SIZE = 100;
    private static readonly HttpClient Client = new();

    private readonly ILogger<PaletteGeneratorService> _logger;

    public PaletteGeneratorService(ILogger<PaletteGeneratorService> logger)
    {
        _logger = logger;
    }
    
    public async Task<Palette> GeneratePalette(string url, int maxPaletteLength)
    {
        try
        {
            var stream = await Client.GetStreamAsync(url);
            var memStream = new MemoryStream();

            await stream.CopyToAsync(memStream);
            memStream.Seek(0, SeekOrigin.Begin);

            var image = SKBitmap.Decode(memStream);

            var resultColors = new List<(int, Color)>();
            var cluster = new int[SAMPLING_RATE, SAMPLING_RATE, SAMPLING_RATE, 4];
            var pixels = image.Pixels;
            var pixelsPerSample = pixels.Length / (SAMPLING_RATE * SAMPLING_RATE);

            var computeAction = new Action<int, int>((beginAt, size) =>
            {
                for (var i = beginAt; i < beginAt + size; i++)
                {
                    var x = (int)(pixels[i].Red / 256.0f * SAMPLING_RATE);
                    var y = (int)(pixels[i].Green / 256.0f * SAMPLING_RATE);
                    var z = (int)(pixels[i].Blue / 256.0f * SAMPLING_RATE);
                    
                    cluster[x, y, z, 0] += pixels[i].Red;
                    cluster[x, y, z, 1] += pixels[i].Green;
                    cluster[x, y, z, 2] += pixels[i].Blue;
                    cluster[x, y, z, 3]++;
                }
            });

            var computeTasks = new List<Task>();

            for (var x = 0; x < pixels.Length; x += COMPUTE_REGION_SIZE)
            {
                var size = Math.Min(COMPUTE_REGION_SIZE, pixels.Length - x);
                var beginAt = x;
                computeTasks.Add(Task.Run(() => computeAction(beginAt, size)));
            }

            await Task.WhenAll(computeTasks);
            
            for (var z = 0; z < SAMPLING_RATE; z++)
            {
                for (var y = 0; y < SAMPLING_RATE; y++)
                {
                    for (var x = 0; x < SAMPLING_RATE; x++)
                    {
                        var n = cluster[x, y, z, 3];
                        if (n == 0)
                            continue;
                        var color = new Color(
                            (byte) (cluster[x, y, z, 0] / n), 
                            (byte) (cluster[x, y, z, 1] / n),
                            (byte) (cluster[x, y, z, 2] / n));
                        resultColors.Add((n, color));
                    }
                }
            }

            var result = new Palette(
                resultColors
                    .OrderByDescending(x => x.Item1)
                    .Take(maxPaletteLength)
                    .Where(x => x.Item1 > pixelsPerSample / 100)
                    .Select(x => x.Item2));

            await stream.DisposeAsync();
            await memStream.DisposeAsync();

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unable to generate a palette from a url");
            return new Palette();
        }
    }
}