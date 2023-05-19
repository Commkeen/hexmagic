using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UICard : MonoBehaviour
{
    public Spell spell;
    public InteractModeState interactMode;
    public TextMeshProUGUI nameTxt;
    public TextMeshProUGUI costTxt;


    private Button _button;

    void Awake()
    {
        _button = GetComponent<Button>();
    }

    void Update()
    {
        _button.enabled = spell.CanCastSpell();
        nameTxt.text = spell.spellName;
        costTxt.text = spell.cost.ToString();
    }

    public void OnClick()
    {
        spell.BeginTargeting();
    }
}
