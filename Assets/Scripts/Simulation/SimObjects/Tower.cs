using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    public const int EXPLORE_RANGE = 4;
    public const int MIASMA_RANGE = 3;

    public float power = 100;

    void FixedUpdate()
    {
        if (power > 100) {power -= Time.fixedDeltaTime;}
    }

    public bool IsPowered()
    {
        return power > 0;
    }

    public void RefreshPower()
    {
        power = 100;
    }
}
