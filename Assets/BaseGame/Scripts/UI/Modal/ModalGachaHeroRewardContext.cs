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
//using UGUI.Core.Modals;
using Game.Data;
using Game.Manager;
using Game.GlobalConfig;
using DG.Tweening;
using Combat;
using TW.UGUI.Core.Activities;

[Serializable]
public class ModalGachaHeroRewardContext 
{
    public static class Events
    {
        public static Subject<Unit> SampleEvent { get; set; } = new();
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> SampleValue { get; private set; }
        [field: SerializeField] public List<SummonHeroReward> ListReward { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            ListReward = (List<SummonHeroReward>)args.Span[0];
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Button BtnClose {get; private set;}
        [field: SerializeField] public MiniPool<UIHeroInventory> PoolUIHeroInventory {get; private set;} = new();
        [field: SerializeField] public UIHeroInventory UIHeroInventory {get; private set;}
        [field: SerializeField] public Transform TfUIContainer {get; private set;}
        [field: SerializeField] public List<UIHeroInventory> ListUIHero {get; private set;} = new();
        
        public UniTask Initialize(Memory<object> args)
        {
            PoolUIHeroInventory.OnInit(UIHeroInventory, 10, TfUIContainer);
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            View.BtnClose.SetOnClickDestination(OnClickBtnClose); 

            SetUpUI();
        }
        private void SetUpUI()
        {
            for (int i = 0; i < Model.ListReward.Count; i++)
            {
                if(Model.ListReward[i].Synergy == Family.None) continue;
                if(IsContainHero(Model.ListReward[i].RewardHero.HeroId)) continue;
                UIHeroInventory uiHeroInventory = View.PoolUIHeroInventory.Spawn(View.TfUIContainer.position, Quaternion.identity);
                uiHeroInventory.Setup(Model.ListReward[i].RewardHero.HeroId);
                View.ListUIHero.Add(uiHeroInventory);
            }
        }
        private bool IsContainHero(int heroID)
        {
            for (int i = 0; i < View.ListUIHero.Count; i++)
            {
                if (View.ListUIHero[i].HeroConfigData.HeroId == heroID)
                {
                    return true;
                }
            }
            return false;
        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }
    }
}