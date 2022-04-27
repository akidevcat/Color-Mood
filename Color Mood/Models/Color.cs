namespace Color_Mood.Models;

public struct Color
{
    public const float LIGHT_LUMINANCE_THRESHOLD = 0.3f;

    public static readonly Color Red = new(255, 0, 0);
    
    public static readonly Color Green = new(0, 255, 0);
    
    public static readonly Color Blue = new(0, 0, 255);
    
    public static readonly Color White = new(255, 255, 255);
    
    public static readonly Color Black = new();
    
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }
    public byte A { get; set; }

    public Color(byte r = 0x0, byte g = 0x0, byte b = 0x0, byte a = 0xFF)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(byte[] rgba)
    {
        if (rgba.Length != 4)
            throw new ArgumentException("Incorrect array length, expected 4", nameof(rgba));

        R = rgba[0];
        G = rgba[1];
        B = rgba[2];
        A = rgba[3];
    }

    public bool IsLight => CalculateLuminance() > LIGHT_LUMINANCE_THRESHOLD;

    public float CalculateLuminance()
    {
        var vR = R / 255.0f;
        var vG = G / 255.0f;
        var vB = B / 255.0f;

        vR = ConvertToLinear(vR);
        vG = ConvertToLinear(vG);
        vB = ConvertToLinear(vB);

        var l = vR * 0.2126f + vG * 0.7152f + vB * 0.0722f;
        
        return l;
    }
    
    public override string ToString()
    {
        return $"{R:X2}{G:X2}{B:X2}";
    }

    private float ConvertToLinear(float channelValue)
    {
        if (channelValue <= 0.04045f)
        {
            return channelValue / 12.92f;
        }
        else
        {
            return MathF.Pow((channelValue + 0.055f) / 1.055f, 2.4f);
        }
    }
}