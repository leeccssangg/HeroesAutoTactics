using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.MVPPattern;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.UGUI.Core.Views;
using UnityEngine.UI;
using Pextension;
using TW.UGUI.Core.Sheets;
using UnityEditor.SceneManagement;
using Game.Data;
using Game.Manager;
using Game.GlobalConfig;
using DG.Tweening;
using Combat;
using TW.UGUI.Core.Activities;

[Serializable]
public class ModalGachaHeroContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
        public static Action<SummonHeroReward> SpawnUIGachaReward { get; set; }
        public static Action CheckCompletedAnim { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<Family> Family { get; set; }
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        [field: SerializeField] public ReactiveValue<int> StageSummon { get; private set; }
        [field: SerializeField] public ReactiveValue<int> NumSummoned { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
            HeroManager.Instance.SetCurrentStageSummon(SummonHeroGlobalConfig.Instance.StartStage);
            StageSummon = HeroManager.Instance.CurStageSummon;
            NumSummoned = new(0);
            Family = HeroManager.Instance.GachaSynergy;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set; }

        [field: SerializeField] public List<UIHeroGacha> ListUIHeroGacha { get; private set; }
        [field: SerializeField] public List<UIGachaReward> ListUIGachaReward { get; private set; }

        [field: SerializeField] public GameObject PanelGachaButton { get; private set; }
        [field: SerializeField] public GameObject PanelGachaReward { get; private set; }
        [field: SerializeField] public GameObject PanelGachaLocked { get; private set; }

        [field: SerializeField] public Transform TfUIHeroGachaContainer { get; private set; }
        [field: SerializeField] public Transform TfUIGachaRewardContainer { get; private set; }
        [field: SerializeField] public Transform TfStartPos { get; private set; }
        [field: SerializeField] public Transform TfEndPos { get; private set; }
        [field: SerializeField] public Transform TfFailedPos { get; private set; }

        [field: SerializeField] public Button BtnGacha10 { get; private set; }
        [field: SerializeField] public Button BtnGacha1 { get; private set; }
        [field: SerializeField] public Button BtnClose { get; private set; }
        [field: SerializeField] public Button BtnFree { get; private set; }

        [field: SerializeField] public TextMeshProUGUI TxtNumSummon { get; private set; }
        [field: SerializeField] public UISelectedPack UISelectedPack { get; private set; }

        
        public UniTask Initialize(Memory<object> args)
        {
            //PoolUIGachaReward.OnInit(UIGachaRewardPrefab, 10, TfUIGachaRewardContainer);
            //PoolUIHeroGacha.OnInit(UIHeroGachaPrefab,10, TfUIHeroGachaContainer);
            ActiveFalseAllUI();
            UISelectedPack.SetInterractAbleButton(true);
            return UniTask.CompletedTask;
        }
        public void ActiveFalseAllUI()
        {
           foreach (UIHeroGacha ui in ListUIHeroGacha)
            {
                ui.gameObject.SetActive(false);
            }
            foreach (UIGachaReward ui in ListUIGachaReward)
            {
                ui.gameObject.SetActive(false);
                ui.PlayAnim(false);
            }
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();
        [field: SerializeField] public int NumHeroAppearance { get; private set; }
        [field: SerializeField] public List<SummonHeroConfig> ListSummonHeroConfig { get; private set; }
        [field: SerializeField] public List<SummonHeroReward> ListSummonHeroReward { get; private set; }
        [field: SerializeField] public int NumGacha { get; private set; }
        [field: SerializeField] public int NumAnimCompleted { get; private set; }
        [field: SerializeField] public List<SummonHeroReward> ListReward { get; private set; }


        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            HeroManager.Instance.SetCurrentStageSummon(SummonHeroGlobalConfig.Instance.StartStage);
            Model.NumSummoned.ReactiveProperty.CombineLatest(Model.StageSummon.ReactiveProperty, (nu, st) => (nu, st))
                .Subscribe(_ => UpdateTxtSummoned()).AddTo(View.MainView);

            Model.Family.ReactiveProperty.Subscribe(OnUpdateSynergy).AddTo(View.MainView);

            Events.SpawnUIGachaReward = SpawnUIGachaReward;
            Events.CheckCompletedAnim = CheckAllAnimCompleted;

            View.BtnGacha10.SetOnClickDestination(OnClickBtnGacha10);
            View.BtnGacha1.SetOnClickDestination(OnClickBtnGacha1);
            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
            View.BtnFree.SetOnClickDestination(OnClickBtnFree);
            View.BtnFree.gameObject.SetActive(false);
            ListReward = new();
        }
        public UniTask Cleanup(Memory<object> args)
        {
            Events.SpawnUIGachaReward = null;
            Events.CheckCompletedAnim = null;
            return UniTask.CompletedTask;
        }
        public void DidPushEnter(Memory<object> args)
        {
            //foreach(UIGachaReward ui in View.ListUIGachaReward)
            //{
            //    ui.PlayAnim(false);
            //}
        }
        private void OnUpdateSynergy(Family synergy)
        {
            View.UISelectedPack.Setup(synergy, OnSelectSynergy);
            //UpdateUIHeroInventory();
            View.PanelGachaButton.SetActive(HeroManager.Instance.IsGachaAbleFamily());
            View.PanelGachaLocked.SetActive(!HeroManager.Instance.IsGachaAbleFamily());
            View.PanelGachaReward.SetActive(false);
        }
        private void OnClickBtnGacha10(Unit _)
        {
            StartGacha(10);
        }
        private void OnClickBtnGacha1(Unit _)
        {
            StartGacha(1);
        }
        private void OnClickBtnFree(Unit _)
        {
            //View.PoolUIGachaReward.Collect();
            //View.PoolUIHeroGacha.Collect();
            View.BtnFree.gameObject.SetActive(false);
            HeroManager.Instance.SetCurrentStageSummon(Model.StageSummon.Value + 1);
            Model.NumSummoned.Value = 0;
            StartGacha(NumGacha);
        }
        private void OnClickBtnClose(Unit _)
        {
            ActivitySelectHeroSynergyGachaContext.Events.HideActivity?.Invoke();
            View.MainView.interactable = false;
            ClickButtonCloseComplete().Forget();
        }
        private async UniTask ClickButtonCloseComplete()
        {
            await ModalContainer.Find(ContainerKey.Modals).PopAsync(true);
            if (ListReward.Count > 0)
            {
                ViewOptions options = new ViewOptions(nameof(ModalGachaHeroReward));
                await ModalContainer.Find(ContainerKey.Modals).PushAsync(options, ListReward);
                HeroManager.Instance.ClaimSummonReward(ListReward);
            }
            
        }
        private void OnNumSummonedChange(int num)
        {
            View.TxtNumSummon.SetText($"{num}/{Model.StageSummon}");
        }
        private void StartGacha(int numGacha)
        {
            //View.PoolUIHeroGacha.Release();
            //View.PoolUIGachaReward.Release();
            View.UISelectedPack.SetInterractAbleButton(false);
            NumGacha = numGacha;
            NumAnimCompleted = 0;
            View.ActiveFalseAllUI();
            View.PanelGachaButton.SetActive(false);
            View.PanelGachaLocked.SetActive(false);
            View.PanelGachaReward.SetActive(true);
            View.BtnClose.interactable = false;
            NumHeroAppearance = HeroManager.Instance.GetNumAppearance();
            ListSummonHeroConfig = HeroManager.Instance.GetListSummonHeroConfig(NumHeroAppearance);
            ListSummonHeroReward = HeroManager.Instance.GetListSummonHeroReward(ListSummonHeroConfig,numGacha);
            //NumAnimCompleted = GetNumAccquired();
            foreach (SummonHeroReward reward in ListSummonHeroReward)
            {
                if(reward.IsAcquired)
                    ListReward.Add(reward);
            }
            
            InitViewGacha();
        }
        private void InitViewGacha()
        {
            //View.PoolUIHeroGacha.Collect();
            //View.PoolUIGachaReward.Collect();
            //foreach()
            //foreach (UIGachaReward ui in View.ListUIGachaReward)
            //{
            //    ui.gameObject.SetActive(false);
            //}
            for (int i = 0;i < ListSummonHeroReward.Count; i++)
            {
                SummonHeroReward reward = ListSummonHeroReward[i];
                UIHeroGacha uiHeroGacha = View.ListUIHeroGacha[i];
                if (reward.IsAcquired)
                    uiHeroGacha.Setup(reward,View.TfStartPos, View.TfEndPos);
                else
                    uiHeroGacha.Setup(reward, View.TfStartPos, View.TfFailedPos);
                //if (i == ListSummonHeroReward.Count - 1)
                //    uiHeroGacha.SetupActionLastSummoned(OnGachaCompleted);
                uiHeroGacha.gameObject.SetActive(true);
                uiHeroGacha.InitDelay(i * 0.35f);
            }
        }
        //private bool IsAllAnimCompleted()
        //{
        //    for (int i = 0; i < View.ListUIHeroGacha.Count; i++)
        //    {
        //        if (ListSummonHeroReward[i].Is)
        //            return false;
        //    }
        //}
        private void CheckAllAnimCompleted()
        {
            NumAnimCompleted += 1;
            if (NumAnimCompleted == ListSummonHeroConfig.Count)
            {
                OnGachaCompleted();
            }
        }
        private void SpawnUIGachaReward(SummonHeroReward summonHeroReward)
        {
            Model.NumSummoned.Value += 1;
            int uiIndex = Model.NumSummoned.Value-1;
            UIGachaReward uiGachaReward = View.ListUIGachaReward[uiIndex];
            uiGachaReward.Setup(summonHeroReward);
            uiGachaReward.gameObject.SetActive(true);
            uiGachaReward.PlayAnim(true);
            //View.ListUIGachaReward.Add(uiGachaReward);
        }
        private void OnGachaCompleted()
        {
            if(Model.StageSummon.Value < 10 && Model.NumSummoned.Value >= Model.StageSummon.Value)
            {
                View.UISelectedPack.SetInterractAbleButton(false);
                View.BtnFree.gameObject.SetActive(true);
                View.BtnClose.interactable = true;
            }
            else
            {
                DOVirtual.DelayedCall(3f, () =>
                {
                    View.PanelGachaButton.gameObject.SetActive(HeroManager.Instance.IsGachaAbleFamily());
                    View.PanelGachaLocked.gameObject.SetActive(!HeroManager.Instance.IsGachaAbleFamily());
                    View.PanelGachaReward.gameObject.SetActive(false);
                    HeroManager.Instance.SetCurrentStageSummon(SummonHeroGlobalConfig.Instance.StartStage);
                    Model.NumSummoned.Value = 0;
                    View.BtnClose.interactable = true;
                    View.UISelectedPack.SetInterractAbleButton(true);
                });
            }

        }
        private void UpdateTxtSummoned()
        {
            View.TxtNumSummon.SetText($"{Model.NumSummoned.Value}/{Model.StageSummon.Value}");
        }
        private void OnSelectSynergy(Family synergy)
        {
            ViewOptions options = new ViewOptions(nameof(ActivitySelectHeroSynergyGacha));
            ActivityContainer.Find(ContainerKey.Activities).ShowAsync(options);
        }

    }
}