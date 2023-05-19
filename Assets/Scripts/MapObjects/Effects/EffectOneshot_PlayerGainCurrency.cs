using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectOneshot_PlayerGainCurrency : EffectOneshot
{
    public int manaGained;

    public CurrencyState playerCurrency;

    public override void Invoke()
    {
        playerCurrency.AddMana(manaGained);
        OnComplete();
    }
}
