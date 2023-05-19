using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_AddEffect : MonoBehaviour
{
    public MonoBehaviour effect;

    public void Invoke()
    {
        gameObject.AddComponent(effect.GetType());
    }
}
