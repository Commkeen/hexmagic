using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MapGenerator : ScriptableObject
{
    public TileFilter tileFilter;

    public TileState Generate(HexCoordinates coords)
    {
        var tile = new TileState(coords);
        tileFilter.Evaluate(tile);
        tile.init = true;
        if (!Application.isPlaying)
        {
            tile.explored = true;
            tile.miasma = false;
        }
        return tile;
    }
}
