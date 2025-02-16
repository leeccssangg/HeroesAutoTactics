using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Game.Manager;
using TW.Reactive.CustomComponent;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using Combat;

public class UISelectedPack : MonoBehaviour
{
    [field: SerializeField] public Family Synergy { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtName { get; private set; }
    [field: SerializeField] public Image ImgSelected { get; private set; }
    [field: SerializeField] public Button BtnSelect { get; private set; }
    [field: SerializeField] public Image ImgUnlockedSynergy { get; private set; }
    [field: SerializeField] public Image ImgLockedSynergy { get; private set; }
    private Action<Family> OnSelectSynergy { get; set; }

    private void Awake()
    {
        BtnSelect.SetOnClickDestination(OnClickBtnSelect);
    }
    public void Setup(Family synergy, Action<Family> action)
    {
        Synergy = synergy;
        OnSelectSynergy = action;
        TxtName.SetText(Synergy.ToString());
        //ImgSelected.gameObject.SetActive(HeroManager.Instance.InventorySynergy.Value == Synergy);
        ImgUnlockedSynergy.gameObject.SetActive(HeroManager.Instance.IsUnlockSynergy(Synergy));
        ImgLockedSynergy.gameObject.SetActive(!HeroManager.Instance.IsUnlockSynergy(Synergy));
        //BtnSelect.interactable = HeroManager.Instance.IsUnlockSynergy(Synergy);
    }
    private void OnClickBtnSelect(Unit _)
    {
        OnSelectSynergy?.Invoke(Synergy);
    }
    public void SetInterractAbleButton(bool isUnlock)
    {
        BtnSelect.interactable = isUnlock;
    }

}
