using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapCollections/MapObjectCollection")]
public class MapObjectCollection : ScriptableObject
{
    public MapObjectEvent onAdded;
    public GameEvent onUpdate;

    private Dictionary<HexCoordinates, List<MapObject>> _collection = new Dictionary<HexCoordinates, List<MapObject>>();

    public void Add(MapObject obj)
    {
        Debug.Assert(obj.GetComponent<MapPosition>().OnMap);
        var coords = obj.WorldCoordinates;
        if (!_collection.ContainsKey(coords)) {_collection[coords] = new List<MapObject>();}
        _collection[coords].Add(obj);
        //Debug.Log($"Added mapobject {obj.gameObject} to collection {name} at coords {coords}");
        onAdded?.Invoke(obj);
        onUpdate?.Invoke();
    }

    public List<MapObject> Get(HexCoordinates coords)
    {
        if (!_collection.ContainsKey(coords)) {_collection[coords] = new List<MapObject>();}
        return _collection[coords];
    }

    public List<MapObject> GetAll()
    {
        var result = new List<MapObject>();
        foreach (var v in _collection)
        {
            result.AddRange(v.Value);
        }
        return result;
    }

    public void Remove(MapObject obj)
    {
        var coords = obj.WorldCoordinates;
        if (_collection[coords] == null) {return;}
        _collection[coords].Remove(obj);
        onUpdate?.Invoke();
    }
}
