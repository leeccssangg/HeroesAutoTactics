using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TMPro;
using DG.Tweening;

[Serializable]
public class ModalDailyGiftContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Func<int,UniTask> ClaimDailyGift { get; set; }
        public static Func<UniTask> ClaimDailyGiftStage { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> NumCollected { get; private set; } = new(0);
        [field: SerializeField] public ReactiveValue<int> Stage { get; private set; } = new(0);
        public ReactiveValue<TimeSpan> TimeToReset { get; private set; } = new(TimeSpan.Zero);

        public UniTask Initialize(Memory<object> args)
        {
            //CurrentDayId = InGameDataManager.Instance.InGameData.DailyGiftData.Id;
            NumCollected = DailyGiftManager.Instance.NumCollected;
            TimeToReset = DailyGiftManager.Instance.TimeToReset;
            Stage = DailyGiftManager.Instance.Stage;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView { get; private set; }
        [field: SerializeField] public Button BtnClose { get; private set; }   
        [field: SerializeField] public List<UIDailyRewardInfo> ListUIDailyRewardInfo { get; private set; }
        [field: SerializeField] public List<UIDailyRewardStage> ListUIDailyRewardStage { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtNumCollected { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtTimeReset { get; private set; }
        [field: SerializeField] public Slider SliderProcess { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            //UIResourceCoin.Initialize(args);
            return UniTask.CompletedTask;
        }
        public void SetupUI()
        {
            //Debug.Log(currentDayId);
            List<DailyReward> listGiftsConfigs = DailyGiftManager.Instance.GetListDailyGifts();
            for (int i = 0; i < ListUIDailyRewardInfo.Count; i++)
            {
                DailyReward gift = listGiftsConfigs[i];
                ListUIDailyRewardInfo[i].Setup(gift);
            }
            SetupUIStage();
        }
        public void SetupUIStage()
        {
            List<DailyGiftStage> dailyGiftStages = DailyGiftManager.Instance.GetListDailyGiftStages();
            for (int i = 0; i < ListUIDailyRewardStage.Count; i++)
            {
                DailyGiftStage gift = dailyGiftStages[i];
                ListUIDailyRewardStage[i].Setup(gift);
            }
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model { get; private set; } = new();
        [field: SerializeField] public UIView View { get; set; } = new();

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            Model.NumCollected.ReactiveProperty.Subscribe(OnNumCollectedChanged).AddTo(View.MainView);
            Model.TimeToReset.ReactiveProperty.Subscribe(OnTimeToResetChange).AddTo(View.MainView);
            Model.Stage.ReactiveProperty.Subscribe(OnStageChange).AddTo(View.MainView);

            View.BtnClose.SetOnClickDestination(OnClose);
            Events.ClaimDailyGift = OnClaimDailyGift;
            Events.ClaimDailyGiftStage = OnCliamDailyGiftStage;
            Debug.Log(DailyGiftManager.Instance.GetCurrentDayId());
        }
        public UniTask Cleanup()
        {
            Events.ClaimDailyGift = null;
            Events.ClaimDailyGiftStage = null;
            return UniTask.CompletedTask;
        }
        private void OnNumCollectedChanged(int num)
        {
            //View.UIResourceCoin.SetAmount(num);
            View.TxtNumCollected.SetText($"{num}");
            View.SliderProcess.DOValue(DailyGiftManager.Instance.GetProcess(), 0.5f);
            View.SetupUI();
        }
        private void OnStageChange(int stage)
        {
            View.SetupUIStage();
        }
        private void OnTimeToResetChange(TimeSpan time)
        {
            View.TxtTimeReset.SetText($"Time reset: {DailyGiftManager.Instance.GetTimeToResetReward()}");
        }
        private async UniTask OnClaimDailyGift(int dayId)
        {
            //VibrationManager.Instance.CallHaptic();
            //List<GameResource> gift = DailyGiftManager.Instance.GetListDailyGift();
            //ViewOptions view = new ViewOptions(nameof(ScreenItemReward));
            //Action actionCallBack = OnClaimDailyGiftCompleted;
            //ScreenContainer.Find(ContainerKey.Screens).PushAsync(view, gift, actionCallBack);
            //PlayerResource.ClaimListReward(DailyGiftManager.Instance.GetListDailyGift(),1);
            await OnClaimDailyGiftCompleted(dayId);
            //DailyGiftManager.Instance.OnClaimComplete();
        }
        private async UniTask OnClaimDailyGiftCompleted(int dayId)
        {
            await DailyGiftManager.Instance.OnClaimComplete(dayId);
        }
        private async UniTask OnCliamDailyGiftStage()
        {
            await DailyGiftManager.Instance.OnClaimStageReward();
        }
        private void OnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }
    }
}