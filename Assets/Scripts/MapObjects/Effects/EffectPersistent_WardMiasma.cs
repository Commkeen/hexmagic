using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MapObject))]
public class EffectPersistent_WardMiasma : MonoBehaviour
{
    public int radius;

    public MapObjectCollection miasmaWards;

    void OnDisable()
    {
        OnDeactivateEffect();
    }

    public void OnActivateEffect()
    {
        miasmaWards.Add(GetComponent<MapObject>());
    }

    public void OnDeactivateEffect()
    {
        miasmaWards.Remove(GetComponent<MapObject>());
    }
}
