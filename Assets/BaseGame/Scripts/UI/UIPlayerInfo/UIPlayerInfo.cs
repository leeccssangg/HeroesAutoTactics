using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine.UI;
using R3;
using TMPro;
using TW.UGUI.MVPPattern;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using Game.Manager;
public class UIPlayerInfo : ACachedMonoBehaviour, IAView
{
    [field: SerializeField] public TextMeshProUGUI TxtUserName { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtLevel { get; private set; }
    [field: SerializeField] public TextMeshProUGUI TxtRank { get; private set; }

    [field: SerializeField] public Image ImgIcon { get; private set; }
    [field: SerializeField] public Image ImgRank { get; private set; }
    [field: SerializeField] public Image ImgFrame { get; private set; }

    [field: SerializeField] public Slider SliderExp { get; private set; }

    public UniTask Initialize(Memory<object> args)
    {
        PlayerProfileManager.Instance.UserName.ReactiveProperty.Subscribe(OnUserNameChanged).AddTo(this);
        //InGameDataManager.Instance.UserData.Rank.ReactiveProperty.Subscribe(OnUserRankChanged).AddTo(this);
        PlayerProfileManager.Instance.Rank.ReactiveProperty.Subscribe(OnUserRankChanged).AddTo(this);
        return UniTask.CompletedTask;
    }
    private void OnUserNameChanged(string userName)
    {
        TxtUserName.SetText($"{userName}");
    }
    private void OnUserRankChanged(Rank rank)
    {
        TxtRank.SetText($"{rank}");
    }
    private void OnUserExpChanged()
    {

    }
    private void OnUserLevelChanged()
    {

    }
}
