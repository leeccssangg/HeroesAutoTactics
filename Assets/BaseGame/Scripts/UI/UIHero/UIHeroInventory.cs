using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using Game.Manager;
using Game.Data;
using TW.Reactive.CustomComponent;
using Cysharp.Threading.Tasks;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Modals;

public class UIHeroInventory : MonoBehaviour
{
    [field: SerializeField] public HeroConfigData HeroConfigData { get; private set; }
    [field: SerializeField] public EachHeroUpgradeData HeroUpgradeData { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtLevel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtPiece { get; private set; }
    [field: SerializeField] public Image ImgRarity { get; private set; }
    [field: SerializeField] public Image ImgHeroIcon { get; private set; }
    [field: SerializeField] public Slider SliderExp { get; private set; }
    [field: SerializeField] public Button BtnInfo { get; private set; }
    [field: SerializeField] public string FormatText { get; private set;}
    private IDisposable _disposableLevel;
    private IDisposable _disposablePiece;

    private void Awake()
    {
        BtnInfo.SetOnClickDestination(OnClickBtnInfo);
    }
    public void Setup(int heroId)
    {
        HeroConfigData = HeroManager.Instance.GetHeroConfigData(heroId);
        HeroUpgradeData = HeroManager.Instance.GetHeroUpgradeData(HeroConfigData.HeroId);
        TxtName.SetText(HeroConfigData.HeroName);
        TxtLevel.SetText(HeroUpgradeData.Level.ToString());
        TxtPiece.SetText(HeroUpgradeData.Piece.ToString());
        //ImgRarity.sprite = HeroConfigData.RarityIcon;
        ImgHeroIcon.sprite = HeroConfigData.SpriteIcon;
        if(HeroUpgradeData.Level >= 1)
        {
            SliderExp.gameObject.SetActive(true);
            SliderExp.value = HeroManager.Instance.GetHeroPieceProcess(HeroConfigData.HeroId);
        }
        else
        {
            SliderExp.gameObject.SetActive(false);
        }

        _disposableLevel?.Dispose();
        _disposablePiece?.Dispose();
        _disposableLevel = HeroUpgradeData.Level.ReactiveProperty.Subscribe(OnLevelHeroChange).AddTo(this);
        _disposablePiece = HeroUpgradeData.Piece.ReactiveProperty.Subscribe(OnPieceHeroChange).AddTo(this);
    }
    private void OnLevelHeroChange(int level)
    {
        TxtLevel.SetText(level.ToString());
    }
    private void OnPieceHeroChange(int piece)
    {
        if(HeroUpgradeData.Level < 1)
        {
            SliderExp.gameObject.SetActive(false);
            return;
        }
        SliderExp.gameObject.SetActive(true);
        SliderExp.value = HeroManager.Instance.GetHeroPieceProcess(HeroConfigData.HeroId);
        TxtPiece.SetText($"{piece}/{HeroManager.Instance.GetPieceNeededUpgradeHero(HeroConfigData.HeroId)}");
    }
    private void OnClickBtnInfo(Unit _)
    {
        ViewOptions option = new ViewOptions(nameof(ModalHeroInfo));
        ModalContainer.Find(ContainerKey.Modals).PushAsync(option, HeroConfigData.HeroId);
    }

}
