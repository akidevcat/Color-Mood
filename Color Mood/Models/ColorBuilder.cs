namespace Color_Mood.Models;

public class ColorBuilder
{
    private int _pointer;
    private readonly byte[] _bytes;

    public ColorBuilder()
    {
        _bytes = new byte[4];
    }

    public ColorBuilder Append(byte channelValue)
    {
        if (_pointer >= _bytes.Length)
            throw new IndexOutOfRangeException("Cannot append new value - builder already is complete");
        _bytes[_pointer++] = channelValue;
        return this;
    }

    public Color Build()
    {
        return new Color(_bytes);
    }
    
    public void Clear()
    {
        Array.Clear(_bytes);
        _pointer = 0;
    }

    public bool IsComplete(bool considersAlphaChannel) => considersAlphaChannel ? _pointer >= 4 : _pointer >= 3;
}