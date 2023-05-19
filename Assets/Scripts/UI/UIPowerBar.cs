using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPowerBar : MonoBehaviour
{

    public SpriteRenderer bg;
    public SpriteRenderer fill;

    private float _value;
    public float Value
    {
        get {return _value;}
        set {SetValue(value);}
    }

    public void SetValue(float value)
    {
        _value = value;
        var max = bg.size.x;
        var min = 4f;
        var size = fill.size;
        size.x = Mathf.Lerp(min, max, value);
        fill.size = size;
        var fillPos = fill.transform.position;
        fillPos.x = Mathf.Lerp((-max/2f)+2, 0, value);
        fill.transform.position = fillPos;
        fill.enabled = value > 0;
    }
}
