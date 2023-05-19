using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Currency/CurrencyState")]
public class CurrencyState : ScriptableObject
{
    public int initialMaxMana = 3;
    public int Mana {get; private set;}
    public int MaxMana {get; private set;}

    void OnEnable()
    {
        MaxMana = initialMaxMana;
    }

    public void AddMana(int amount)
    {
        Mana += amount;
        if (Mana > MaxMana) {Mana = MaxMana;}
    }

    public void RemoveMana(int amount)
    {
        Mana -= amount;
        if (Mana < 0) {Mana = 0;}
    }
}
