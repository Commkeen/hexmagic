using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : IEventListener
{
    public GameEvent gameEvent;
    public UnityEvent onEvent;
    public override void OnEvent()
    {
        onEvent.Invoke();
    }

    protected override GameEvent GetEvent()
    {
        return gameEvent;
    }
}
