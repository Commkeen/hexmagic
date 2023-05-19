using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave
{
    public HexCoordinates origin;
    public float currentTime = 0;
    public float speed = 3f;
    public float distance = 3f;
    public float wavelength = 0.5f;

    public float GetValue(float point)
    {
        var pos = currentTime*speed;
        point = point-pos;
        if (point < 0) {point = 0;}
        if (point > wavelength*2) {return 0;}

        var angle = point*360f;
        return Mathf.Sin(angle*(1f/wavelength));
    }
}
