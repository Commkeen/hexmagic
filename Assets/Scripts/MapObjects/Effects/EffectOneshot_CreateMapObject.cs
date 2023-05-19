using UnityEngine;

[RequireComponent(typeof(MapPosition))]
public class EffectOneshot_CreateMapObject : EffectOneshot
{
    public MapObject mapObject;

    public override void Invoke()
    {
        var coords = GetComponent<MapPosition>().WorldCoordinates;
        var obj = GameObject.Instantiate<MapObject>(mapObject);
        obj.SetCoordinates(coords);
        OnComplete();
    }
}
