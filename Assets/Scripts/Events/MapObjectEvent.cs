using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/MapObjectEvent")]
public class MapObjectEvent : GameEvent
{
    public void Invoke(MapObject obj)
    {
        //Debug.Log($"Invoke mapobjectevent {name} for {_listeners.Count} listeners");
        for (int i = _listeners.Count-1; i >= 0; i--)
        {
            var lst = _listeners[i];
            if (lst is MapObjectEventListener)
            {
                ((MapObjectEventListener)lst).OnEvent(obj);
            }
        }
    }
}
