using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "MapCollections/HexCoordinateSet")]
public class HexCoordinateSet : ScriptableObject
{
    private HashSet<HexCoordinates> _set = new HashSet<HexCoordinates>();

    public void Add(HexCoordinates coords)
    {
        _set.Add(coords);
    }

    public void Remove(HexCoordinates coords)
    {
        _set.Remove(coords);
    }

    public bool Exists(HexCoordinates coords)
    {
        return _set.Contains(coords);
    }

    public IEnumerable<HexCoordinates> GetEnumerable()
    {
        return _set;
    }

    public List<HexCoordinates> GetAllInRadius(HexCoordinates coords, int radius)
    {
        var results = new List<HexCoordinates>();
        var coordsInRadius = coords.GetAllInRadius(radius);
        foreach (var c in coordsInRadius)
        {
            if (Exists(c))
            {
                results.Add(c);
            }
        }
        return results;
    }

    public int Count()
    {
        return _set.Count();
    }

    public HexCoordinates GetRandom()
    {
        var r = Random.Range(0, _set.Count);
        return _set.ElementAt(r);
    }
}
