namespace Color_Mood.Utils;

public static class ParsingUtils
{
    public static IEnumerable<byte> ParseStringToByteArray(string hex)
    {
        if (hex.Length % 2 != 0)
            throw new ArgumentException("String has an incorrect number of characters", nameof(hex));

        return Enumerable.Range(0, hex.Length / 2)
            .Select(i => Convert.ToByte(hex[(i * 2)..((i + 1) * 2)], 16));
    }
}