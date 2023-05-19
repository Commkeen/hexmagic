using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EffectOneshot : MonoBehaviour
{
    public bool runOnInit = false;
    public bool removeOnComplete = false;
    public bool hasCompleted = false;

    protected void OnActivateEffect()
    {
        if (runOnInit)
        {
            Invoke();
        }
    }

    protected void OnComplete()
    {
        hasCompleted = true;
        if (removeOnComplete)
        {
            GameObject.Destroy(this); //component
        }
    }

    public abstract void Invoke();
}
