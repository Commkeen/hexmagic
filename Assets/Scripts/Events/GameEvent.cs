using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Events/GameEvent")]
public class GameEvent : ScriptableObject
{
    protected List<IEventListener> _listeners = new List<IEventListener>();

    public void AddListener(IEventListener listener)
    {
        _listeners.Add(listener);
    }

    public void RemoveListener(IEventListener listener)
    {
        _listeners.Remove(listener);
    }

    public void Invoke()
    {
        for (int i = _listeners.Count-1; i >= 0; i--)
        {
            _listeners[i].OnEvent();
        }
    }
}
