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
using TW.UGUI.Core.Activities;

[Serializable]
public class ModalLoseContext 
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
        [field: SerializeField] public int CupCount { get; private set; }
        [field: SerializeField] public int HeartCount { get; private set; }


        public UniTask Initialize(Memory<object> args)
        {
            CupCount = (int)args.Span[0];
            HeartCount = (int)args.Span[1];
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public List<UICupPlayer> ListUICupHero { get; private set; }
        [field: SerializeField] public List<UIHeartPlayer> ListUIHeartHero { get; private set; }
        [field: SerializeField] public Button BtnClose { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {
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
            SetupUICup();
            SetupUIHeart();
            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
        }
        public void DidPushEnter(Memory<object> args)
        {
            PlayAnimLoseHeart();
            //WaitToChangeBackToGame().Forget();
        }
        private void PlayAnimLoseHeart()
        {
            for (int i = 0; i < View.ListUIHeartHero.Count; i++)
            {
                if (i == Model.HeartCount)
                {
                    View.ListUIHeartHero[i].PlayAnimLoseHeart();
                }
            }
        }
        private void SetupUICup()
        {
            for (int i = 0; i < View.ListUICupHero.Count; i++)
            {
                //View.ListUICupHero[i].Setup(i < Model.CupCount);
                if (i + 1 > Model.CupCount)
                {
                    View.ListUICupHero[i].PlayAnimInitLose();
                }
            }
        }
        private void SetupUIHeart()
        {
            for (int i = 0; i < View.ListUIHeartHero.Count; i++)
            {
                //View.ListUIHeartHero[i].Setup(i < Model.HeartCount);
                if (i + 1 > Model.HeartCount + 1)
                {
                    View.ListUIHeartHero[i].PlayAnimInitLose();
                }
            }
        }
        
        private void OnClickBtnClose(Unit _)
        {
            ViewOptions options = new ViewOptions(nameof(ActivityChangeView));
            Action onChangeViewCloseCallback = OnChangeViewClose;
            Action onChangeViewOpenCallback = OnChangeViewOpen;
            ActivityContainer.Find(ContainerKey.OverlayActivity).Show(options, onChangeViewCloseCallback, onChangeViewOpenCallback, 1f);
        }
        private void OnChangeViewClose()
        {
            ModalContainer.Find(ContainerKey.OverlayModal).Pop(true);
            InGameHandleManager.Instance.ChangeToPrepareState();
        }
        private void OnChangeViewOpen()
        {
            
        }
    }
}