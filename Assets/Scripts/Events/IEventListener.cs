using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IEventListener : MonoBehaviour
{
    void OnEnable()
    {
        GetEvent().AddListener(this);
    }

    void OnDisable()
    {
        GetEvent().RemoveListener(this);
    }

    public abstract void OnEvent();

    protected abstract GameEvent GetEvent();
}
