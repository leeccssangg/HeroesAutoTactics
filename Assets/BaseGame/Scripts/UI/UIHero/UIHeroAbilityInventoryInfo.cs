using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Combat;

public class UIHeroAbilityInventoryInfo : MonoBehaviour
{
    [field: SerializeField] public AbilityPower AbilityPower { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtAbilityLevel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtAbilityDescription { get; private set; }

    public void Setup(AbilityPower abilityPower)
    {
        AbilityPower = abilityPower;
        TxtAbilityLevel.SetText($"{AbilityPower.AbilityLevel} Star");
        TxtAbilityDescription.SetText($"{AbilityPower.Description}");
    }
}
