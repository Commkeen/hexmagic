using UnityEngine;

public class ColorFromTileStateFilter
{
    ColorSettings settings;

    public ColorFromTileStateFilter(ColorSettings settings)
    {
        this.settings = settings;
    }

    public void Evaluate(HexCell cell)
    {
        cell.color = GetColorForTile(cell.Tile);
    }

    private Color GetColorForTile(TileState tile)
    {
        if (!tile.init) {return Color.magenta;}
        if (tile.Elevation == 0) {return settings.water;}
        if (tile.Elevation == 2) {return settings.mountain;}
        if (tile.Life == 0) {return settings.beach;}
        if (tile.Life == 1) {return settings.lowland;}
        if (tile.Life == 2) {return settings.highland;}
        return Color.magenta;
    }
}