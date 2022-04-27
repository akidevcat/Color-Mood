using System.Text;
using Color_Mood.Utils;

namespace Color_Mood.Models;

public class Palette
{
    public List<Color> Colors { get; set; }

    public Palette()
    {
        Colors = new List<Color>();
    }
    
    public Palette(IEnumerable<Color> colors)
    {
        Colors = new List<Color>();
        Colors.AddRange(colors);
    }

    public Palette(params Color[] colors)
    {
        Colors = new List<Color>();
        Colors.AddRange(colors);
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        foreach (var color in Colors)
        {
            builder.Append(color.ToString().ToLower());
        }

        return builder.ToString();
    }

    /// <summary>
    /// byteString format:
    /// 0-2. Color A bytes
    /// 3-5. Color B bytes
    /// 6-8. Color C bytes...
    /// </summary>
    /// <param name="byteString"></param>
    /// <param name="result"></param>
    /// <param name="readAlphaChannel"></param>
    /// <returns></returns>
    public static bool TryParse(string byteString, out Palette result, bool readAlphaChannel = true)
    {
        result = new Palette();
        var bytesPerColor = readAlphaChannel ? 4 : 3;
        var bytesLength = byteString.Length / 2;

        // byteString length is incorrect
        if (byteString.Length % bytesPerColor != 0)
        {
            return false;
        }
        
        try
        {
            var bytes = ParsingUtils.ParseStringToByteArray(byteString);
            var colorBuilder = new ColorBuilder();
            
            foreach (var b in bytes)
            {
                colorBuilder.Append(b);
                
                if (colorBuilder.IsComplete(false))
                {
                    result.Colors.Add(colorBuilder.Build());
                    colorBuilder.Clear();
                }
            }

            return true;
        }
        catch (ArgumentException ex)
        {
            
            
        }
        catch (IndexOutOfRangeException ex)
        {

        }
        
        return false;
    }
}