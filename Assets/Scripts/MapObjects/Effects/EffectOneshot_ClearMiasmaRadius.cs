using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapPosition))]
public class EffectOneshot_ClearMiasmaRadius : EffectOneshot
{
    public int radius;

    public HexCoordinateSet miasmaClearedSet;
    public TileCollection tiles;

    public override void Invoke()
    {
        var coords = GetComponent<MapPosition>().WorldCoordinates;
        var miasmaCoords = coords.GetAllInRadius(radius);
        foreach (var c in miasmaCoords)
        {
            //Debug.Log($"Exploring tile {c}");
            miasmaClearedSet.Add(c);
            tiles.Get(c).miasma = false;
        }
        OnComplete();
    }
}
