using UnityEngine;
using UnityEngine.Events;

public class Powered : MonoBehaviour
{
    [Header("Power")]
    public float powerMax = 100;
    public float powerDrain = 1;
    public float Power {get;set;}

    [Header("Events")]
    public UnityEvent onPowerEmpty;
    public UnityEvent onPowerRestoredFromEmpty;

    [Header("UI")]
    public UIPowerBar uiBar;

    void Start()
    {
        Power = powerMax;
        uiBar.SetValue(Power/powerMax);
    }

    void FixedUpdate()
    {
        if (Power > 0)
        {
            Power -= powerDrain*Time.fixedDeltaTime;
            uiBar.SetValue(Power/powerMax);
            if (Power < 0) {Power = 0;}
            if (Power == 0)
            {
                onPowerEmpty?.Invoke();
            }
        }
    }

    public void Recharge()
    {
        var powerWasEmpty = Power == 0;
        Power = powerMax;
        uiBar.SetValue(Power/powerMax);
        if (powerWasEmpty)
        {
            onPowerRestoredFromEmpty?.Invoke();
        }
    }

}
