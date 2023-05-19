using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Spell : MonoBehaviour
{
    public string spellName;
    public int cost;

    public UnityEvent onCast;
    public InteractModeState interactMode;
    public CurrencyState playerCurrency;

    public void BeginTargeting()
    {
        interactMode.selectedSpell = this;
        interactMode.isSpellTargeting = true;
    }

    public void CancelTargeting()
    {
        interactMode.selectedSpell = null;
        interactMode.isSpellTargeting = false;
    }

    public bool CanCastSpell()
    {
        return playerCurrency.Mana >= cost;
    }

    public void SetTarget(HexCoordinates coords)
    {
        GetComponent<MapPosition>().SetCoordinates(coords);
    }

    public void CastSpell()
    {
        playerCurrency.RemoveMana(cost);
        onCast?.Invoke();
        CancelTargeting();
    }
}
