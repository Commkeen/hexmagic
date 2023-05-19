using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MapCollections/HexCoordinateVar")]
public class HexCoordinateVar : ScriptableObject
{
    public CoordsEvent onChanged;
    public GameEvent onCleared;

    private HexCoordinates? _value;
    public HexCoordinates Value
    {
        get {return _value.Value;}
        set
        {
            _value = value;
            onChanged?.Invoke(value);
        }
    }
    public bool HasValue()
    {
        return _value.HasValue;
    }
    public void Clear()
    {
        _value = null;
        onCleared?.Invoke();
    }
}
