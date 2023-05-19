using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public float healthMax = 100;
    public float healthMiasmaDrain = 20;
    public float Value {get;private set;}

    [Header("Data")]
    public HexCoordinateSet miasmaClearTiles;

    void Start()
    {
        Value = healthMax;
    }

    void FixedUpdate()
    {
        var coords = GetComponent<MapObject>().WorldCoordinates;
        if (!miasmaClearTiles.Exists(coords))
        {
            TakeMiasmaDamage(Time.fixedDeltaTime);
        }
    }

    public void TakeDamage(float damage)
    {
        if (Value <= 0) {return;}
        Value -= damage;
        if (Value <= 0)
        {
            GetComponent<MapObject>().Destroy();
        }
    }

    private void TakeMiasmaDamage(float deltaTime)
    {
        var dmg = healthMiasmaDrain*deltaTime;
        TakeDamage(dmg);
    }
}
