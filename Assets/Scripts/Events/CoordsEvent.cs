using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/CoordsEvent")]
public class CoordsEvent : GameEvent
{
    public void Invoke(HexCoordinates coords)
    {
        //Debug.Log($"Invoke coordsevent {name} at coords {coords} for {_listeners.Count} listeners");
        for (int i = _listeners.Count-1; i >= 0; i--)
        {
            var lst = _listeners[i];
            if (lst is CoordsEventListener)
            {
                ((CoordsEventListener)lst).OnEvent(coords);
            }
        }
    }
}
