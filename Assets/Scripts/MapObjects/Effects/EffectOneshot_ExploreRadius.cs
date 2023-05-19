using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapPosition))]
public class EffectOneshot_ExploreRadius : EffectOneshot
{
    public int radius;

    public HexCoordinateSet exploredTileSet;
    public TileCollection tiles;

    public override void Invoke()
    {
        var coords = GetComponent<MapPosition>().WorldCoordinates;
        var exploreCoords = coords.GetAllInRadius(radius);
        foreach (var c in exploreCoords)
        {
            //Debug.Log($"Exploring tile {c}");
            exploredTileSet.Add(c);
            tiles.Get(c).explored = true;
        }
        OnComplete();
    }
}
