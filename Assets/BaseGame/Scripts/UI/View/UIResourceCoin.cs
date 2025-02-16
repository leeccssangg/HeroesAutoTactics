using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using TW.UGUI.MVPPattern;
using TW.Utility.CustomComponent;
using TW.Utility.CustomType;
using UnityEngine;
using LitMotion;
using Game.Data;
using Game.Manager;


public class UIResourceCoin : ACachedMonoBehaviour, IAView
{
    [field: SerializeField] private UserResource.Type Type { get; set; }
    [field: SerializeField] private string TextFormat { get; set; }
    [field: SerializeField] public TextMeshProUGUI TextCoin { get; private set; }
    private int StartResource { get; set; }
    private int TargetResource { get; set; }
    private int CurrentResource { get; set; }
    private MotionHandle RemapMotion { get; set; }
    private MotionHandle EffectMotion { get; set; }
    private bool IsDelayIncrease { get; set; }
    private int DelayIncreaseValue { get; set; }
    private Vector3 DelayIncreaseValuePos { get; set; }
    [field: SerializeField] public Transform MainView { get; private set; }
    [field: SerializeField] public RectTransform ResourceImage { get; private set; }
    //[field: SerializeField] public UIResourceEffect UIResourceEffect { get; private set; }
    public UniTask Initialize(Memory<object> args)
    {
        TargetResource = InGameDataManager.Instance.UserData.UserResourceList.GetResource(Type).Amount;
        CurrentResource = TargetResource;
        StartResource = TargetResource;
        InGameDataManager.Instance.UserData.UserResourceList.GetResource(Type).Amount.ReactiveProperty.Subscribe(OnResourceChange).AddTo(this);
        return UniTask.CompletedTask;
    }

    private void OnResourceChange(int amount)
    {
        if (IsDelayIncrease)
        {
            IsDelayIncrease = false;
            List<GameObject> list = new List<GameObject>();
            for (int i = 0; i < DelayIncreaseValue; i++)
            {
                //UIResourceEffect ui = Instantiate(UIResourceEffect, DelayIncreaseValuePos, Quaternion.identity, MainView);
                //ui.Setup(DelayIncreaseValuePos, ResourceImage.position);
            }

            EffectMotion.TryCancel();
            EffectMotion = LMotion.Create(0, 1, 1).WithOnComplete(() =>
            {
                OnResourceChangeCompleted(amount);
            })
            .RunWithoutBinding();
        }
        else
        {
            OnResourceChangeCompleted(amount);
        }
    }
    private void OnResourceChangeCompleted(int amount)
    {
        RemapMotion.TryCancel();
        TargetResource = amount;
        StartResource = CurrentResource;
        RemapMotion = LMotion.Create(0f, 1f, 0.5f)
            .WithEase(Ease.Linear)
            .Bind(OnUpdateResource);
    }

    private void OnUpdateResource(float process)
    {
        CurrentResource = ReMap(StartResource, TargetResource, process);
        TextCoin.SetText($"{CurrentResource}");
    }

    private int ReMap(int a, int b, float t)
    {
        return (int)(a + t * (b - a));
    }
    public void SetDelayIncreaseValue(Vector3 pos, int value)
    {
        IsDelayIncrease = true;
        DelayIncreaseValuePos = pos;
        DelayIncreaseValue = value;
    }
}
