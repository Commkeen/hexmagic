using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "MapCollections/TileCollection")]
public class TileCollection : ScriptableObject
{
    public MapGenerator generator;

    private Dictionary<HexCoordinates, TileState> _collection = new Dictionary<HexCoordinates, TileState>();

    public void Add(TileState obj)
    {
        var coords = obj.WorldCoordinates;
        _collection[coords] = obj;
    }

    public TileState Get(HexCoordinates coords)
    {
        if (!_collection.ContainsKey(coords))
        {
            Debug.Assert(generator != null);
            _collection[coords] = generator.Generate(coords);
        }
        return _collection[coords];
    }

    public bool Has(HexCoordinates coords)
    {
        return _collection.ContainsKey(coords);
    }

    public IEnumerable<HexCoordinates> GetCoordinatesEnumerable()
    {
        return _collection.Keys;
    }

    public List<TileState> GetAll()
    {
        return _collection.Values.ToList();
    }

    public void Remove(TileState obj)
    {
        var coords = obj.WorldCoordinates;
        _collection.Remove(coords);
    }
}
