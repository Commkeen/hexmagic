using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimMiasma : MonoBehaviour
{
    public float tickRate = 1;

    public MapObjectCollection miasmaWards;
    public HexCoordinateSet miasmaClearedHexes;
    public HexCoordinateSet exploredTiles;
    public TileCollection tiles;

    private HashSet<HexCoordinates> _wardedHexes;
    private float _tickTimer;
    void FixedUpdate()
    {
        _tickTimer += Time.fixedDeltaTime;
        if (_tickTimer >= tickRate)
        {
            _tickTimer -= tickRate;
            Tick();
        }
    }

    public void OnMiasmaWardsUpdated()
    {
        UpdateWardedHexSet();
    }

    private void Tick()
    {
        if (_wardedHexes == null) {UpdateWardedHexSet();}

        var spreadTiles = new HashSet<HexCoordinates>();
        var clearTiles = new HashSet<HexCoordinates>();
        foreach (var coord in tiles.GetCoordinatesEnumerable())
        {
            if (miasmaClearedHexes.Exists(coord))
            {
                continue;
            }

            if (_wardedHexes.Contains(coord))
            {
                clearTiles.Add(coord);
            }
            else
            {
                foreach (var n in coord.GetAllNeighbors())
                {
                    if (!_wardedHexes.Contains(n))
                    {
                        spreadTiles.Add(n);
                    }
                }
            }
        }

        foreach (var c in spreadTiles)
        {
            if (tiles.Has(c))
            {
                tiles.Get(c).miasma = true;
                miasmaClearedHexes.Remove(c);
            }
        }

        foreach (var c in clearTiles)
        {
            if (tiles.Has(c))
            {
                tiles.Get(c).miasma = false;
                miasmaClearedHexes.Add(c);
            }
        }
    }

    private void UpdateWardedHexSet()
    {
        _wardedHexes = new HashSet<HexCoordinates>();
        var wards = miasmaWards.GetAll();
        foreach (var obj in wards)
        {
            var coords = obj.WorldCoordinates;
            var wardEffect = obj.GetComponent<EffectPersistent_WardMiasma>();
            Debug.Assert(wardEffect != null);
            var warded = coords.GetAllInRadius(wardEffect.radius);
            foreach (var c in warded)
            {
                _wardedHexes.Add(c);
            }
        }
        //Debug.Log($"{_wardedHexes.Count} warded hexes.");
    }
}
