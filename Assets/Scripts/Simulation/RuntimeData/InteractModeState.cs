using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State/Interact Mode State")]
public class InteractModeState : ScriptableObject
{
    public bool isSpellTargeting = false;
    public Spell selectedSpell = null;
}
