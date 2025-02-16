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
using TW.UGUI.Core.Activities;
using Combat;
using TW.Reactive.CustomComponent;

public class SheetHero : Sheet, ISetupAble
{
    public static class Events
    {

    }
    [field: SerializeField] public Family Synergy { get; private set; }
    [field: SerializeField] public UISelectedPack UISelectedPack { get; private set; }
    [field: SerializeField] public List<UIHeroInventory> ListUIHeroInventory { get; private set; }
    [field: SerializeField] public Button BtnSortRarity { get; private set; }

    public override UniTask Initialize(Memory<object> args)
    {
        HeroManager.Instance.InventorySynergy.ReactiveProperty.Subscribe(OnUpdateSynergy).AddTo(this);
        return UniTask.CompletedTask;
    }
    public void Setup()
    {
        //OnUpdateSynergy(HeroManager.Instance.InventorySynergy);
    }
    private void OnUpdateSynergy(Family synergy)
    {
        Synergy  = synergy;
        UISelectedPack.Setup(Synergy, OnSelectSynergy);
        UpdateUIHeroInventory();
    }
    private void UpdateUIHeroInventory()
    {
        List<HeroConfigData> list = HeroManager.Instance.GetListHeroConfigDataBySynergy(Synergy);
        for (int i = 0; i < ListUIHeroInventory.Count; i++)
        {
            if(i >= list.Count)
            {
                ListUIHeroInventory[i].gameObject.SetActive(false);
                continue;
            }
            ListUIHeroInventory[i].gameObject.SetActive(true);
            ListUIHeroInventory[i].Setup(list[i].HeroId);
        }
    }
    private void OnSelectSynergy(Family synergy)
    {
        ViewOptions options = new ViewOptions(nameof(ActivitySelectHeroSynergy));
        ActivityContainer.Find(ContainerKey.Activities).ShowAsync(options);
    }
    private void OnClickBtnSortByRarity(Unit _)
    {
        List<HeroConfigData> list = HeroManager.Instance.GetListHeroConfigDataBySynergy(Synergy);
        list = list.OrderBy(x => x.Rarity).ToList();
        for (int i = 0; i < ListUIHeroInventory.Count; i++)
        {
            if (i >= list.Count)
            {
                ListUIHeroInventory[i].gameObject.SetActive(false);
                continue;
            }
            ListUIHeroInventory[i].gameObject.SetActive(true);
            ListUIHeroInventory[i].Setup(list[i].HeroId);
        }
    }
    private void OnClickBtnSortByLevel()
    {
        //List<HeroConfigData> list = HeroManager.Instance.GetListHeroConfigDataBySynergy(Synergy);
        //list = list.OrderBy(x => x.).ToList();
        //for (int i = 0; i < ListUIHeroInventory.Count; i++)
        //{
        //    if (i >= list.Count)
        //    {
        //        ListUIHeroInventory[i].gameObject.SetActive(false);
        //        continue;
        //    }
        //    ListUIHeroInventory[i].gameObject.SetActive(true);
        //    ListUIHeroInventory[i].Setup(list[i].HeroId);
        //}
    }
}

