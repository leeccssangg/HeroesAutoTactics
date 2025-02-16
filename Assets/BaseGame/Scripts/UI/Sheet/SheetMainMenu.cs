using System.Collections.Generic;
using R3;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Sheets;
using UnityEngine;
using UnityEngine.UI;
using TW.UGUI.Core.Views;
using TW.UGUI.Core.Screens;
using System;
using Pextension;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Linq; 
using Sirenix.Utilities;
using DG.Tweening;
using Game.Manager;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;

public class SheetMainMenu : Sheet, ISetupAble
{
    public static class Events
    {

    }
    [field: SerializeField] public Button BtnPlay { get; private set; }
    [field: SerializeField] public Button BtnGacha { get; private set; }
    [field: SerializeField] public Button BtnProfile { get; private set; }
    [field: SerializeField] public Button BtnDailyGift { get; private set; }

    [field: SerializeField] public UIPlayerInfo UIPlayerInfo { get; private set; }
    [field: SerializeField] public List<UIResourceCoin> ListUIResource { get; private set; }
    public override UniTask Initialize(Memory<object> args)
    {
        UIPlayerInfo.Initialize(args);
        ListUIResource.ForEach(ui => ui.Initialize(args));
        return UniTask.CompletedTask;
    }
    public void Setup()
    {
        BtnPlay.SetOnClickDestination(OnClickBtnPlay);
        BtnGacha.SetOnClickDestination(OnClickBtnGacha);
        BtnProfile.SetOnClickDestination(OnClickBtnProfile);
        BtnDailyGift.SetOnClickDestination(OnClickBtnDailyGift);
    }
    private void OnClickBtnGacha(Unit _)
    {
        ViewOptions options = new ViewOptions(nameof(ModalGachaHero));
        ModalContainer.Find(ContainerKey.Modals).Push(options);
    }
    private void OnClickBtnPlay(Unit _)
    {
        // ViewOptions options = new ViewOptions(nameof(ModalLose));
        // ModalContainer.Find(ContainerKey.Modals).Push(options,5,2);
        GameManager.Instance.StartGame();
    }
    private void OnClickBtnProfile(Unit _)
    {
        ViewOptions options = new ViewOptions(nameof(ModalProfileInfo));
        ModalContainer.Find(ContainerKey.Modals).Push(options);
    }
    private void OnClickBtnDailyGift(Unit _)
    {
        ViewOptions options = new ViewOptions(nameof(ModalDailyGift));
        ModalContainer.Find(ContainerKey.Modals).Push(options);
    }
}
