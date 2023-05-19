using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CoordsEventListener : IEventListener
{
    public bool onlyMyCoords = false;
    public CoordsEvent gameEvent;

    public UnityEvent<HexCoordinates> onEvent;

    public override void OnEvent()
    {

    }

    public void OnEvent(HexCoordinates coords)
    {
        if (!enabled) {return;}
        if (onlyMyCoords)
        {
            var pos = GetComponent<MapPosition>();
            Debug.Assert(pos != null);
            if (pos != null)
            {
                if (coords != pos.WorldCoordinates) {return;}
            }
        }
        onEvent.Invoke(coords);
    }

    protected override GameEvent GetEvent()
    {
        return gameEvent;
    }
}
