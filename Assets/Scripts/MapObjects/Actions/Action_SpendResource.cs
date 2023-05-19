using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action_SpendResource : MonoBehaviour
{
    public int cost = 1;
    public CurrencyState currency;
    public UnityEvent onSuccess;

    public void Invoke()
    {
        if (currency.Mana < cost) {return;}

        currency.RemoveMana(cost);
        onSuccess?.Invoke();
    }
}
