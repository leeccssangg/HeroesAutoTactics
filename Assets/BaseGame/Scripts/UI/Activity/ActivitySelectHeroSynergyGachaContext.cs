using System;
using Cysharp.Threading.Tasks;
using TW.Reactive.CustomComponent;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;
using System.Collections.Generic;
using Pextension;
using R3.Triggers;
using System.Collections;
using TW.Utility.Extension;
using System.Threading;
using TW.UGUI.MVPPattern;
using Game.Manager;
using Combat;

[Serializable]
public class ActivitySelectHeroSynergyGachaContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action HideActivity { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<Family> HeroSynergy { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            HeroSynergy.Value = HeroManager.Instance.GachaSynergy;
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView { get; private set; }
        [field: SerializeField] public MiniPool<UISelectedPack> PoolSelectPack { get; private set; } = new();
        [field: SerializeField] public UISelectedPack UISelectedPack { get; private set; }
        [field: SerializeField] public RectTransform RectTfGoSelectSynergy { get; private set; }
        [field: SerializeField] public Transform TfContainer { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            PoolSelectPack.OnInit(UISelectedPack, Enum.GetNames(typeof(HeroSynergy)).Length - 1, TfContainer);
            return UniTask.CompletedTask;
        }
    }
        [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model { get; private set; } = new();
        [field: SerializeField] public UIView View { get; set; } = new();
        private Family Synergy { get; set; }

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            View.MainView.LateUpdateAsObservable()
               .Subscribe(OnLateUpdate)
               .AddTo(View.MainView);
            Events.HideActivity = HideActivity;
            for (int i = 1; i < Enum.GetNames(typeof(Family)).Length; i++)
            {
                Synergy = (Family)i;
                if (Synergy == HeroManager.Instance.GachaSynergy)
                    continue;
                UISelectedPack uiSelectPack = View.PoolSelectPack.Spawn(View.TfContainer.position, Quaternion.identity);
                uiSelectPack.Setup(Synergy, OnSelectSynergy);
            }
        }
        public UniTask Cleanup(Memory<object> args)
        {
            Events.HideActivity = null;
            return UniTask.CompletedTask;
        }
        public async UniTask WillEnter(Memory<object> args)
        {
            //await OnShowMenu();
        }
        //private async UniTask OnShowMenu()
        //{
        //    View.PoolSelectPack.Collect();
        //}
        private void OnSelectSynergy(Family synergy)
        {
            HeroManager.Instance.ChangeGachaSynergy(synergy);
            HideActivity();
        }
        private void OnLateUpdate(Unit unit)
        {
            //if (!View.GoGiftDetail.activeInHierarchy) return;;
            if (Input.GetMouseButtonDown(0) &&
                !RectTransformUtility.RectangleContainsScreenPoint(View.RectTfGoSelectSynergy, Input.mousePosition, Camera.main))
            {
                HideActivity();
            };
        }
        private void HideActivity()
        {
            ActivityContainer.Find(ContainerKey.Activities).HideAsync(nameof(ActivitySelectHeroSynergyGacha));
        }
    }
}