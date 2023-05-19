
public enum HexDirection {NE, E, SE, SW, W, NW}

public static class HexDirectionExtensions
{
    public static HexDirection Opposite(this HexDirection direction)
    {
        if ((int)direction < 3) {return direction+3;}
        return direction-3;
    }

    public static HexDirection Previous(this HexDirection direction)
    {
        if (direction == HexDirection.NE) {return HexDirection.NW;}
        return direction-1;
    }

    public static HexDirection Next(this HexDirection direction)
    {
        if (direction == HexDirection.NW) {return HexDirection.NE;}
        return direction+1;
    }
}