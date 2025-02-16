using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using TW.UGUI.Core.Views;
using UnityEngine.UI;


[Serializable]
public class ActivityConnectionErrorContext 
{
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        public UniTask Initialize(Memory<object> args)
        {   
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;} 
        [field: SerializeField] public CanvasGroup RootView  { get; private set; }
        [field: SerializeField] public Button ButtonTryAgain {get; private set;}
        [field: SerializeField] public Button ButtonClose {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        private IActivityLifecycleEvent m_ActivityLifecycleEventImplementation;
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();
        private Action OnClickButtonTryAgainCallback { get; set; }
        private MotionHandle MotionHandle { get; set; }
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            
            View.RootView.alpha = 0;
            MotionHandle.TryCancel();
            MotionHandle = LMotion.Create(0f, 1f, 0.2f)
                .Bind(value => View.RootView.alpha = value);
            
            OnClickButtonTryAgainCallback = args.Span[0] as Action;
            View.ButtonTryAgain.SetOnClickDestination(OnClickButtonTryAgain);
            View.ButtonClose.SetOnClickDestination(OnClickButtonClose);
        }      
        public UniTask Cleanup(Memory<object> args)
        {
            MotionHandle.TryCancel();
            return UniTask.CompletedTask;
        }
        public async UniTask OnClickButtonTryAgain()
        {
            View.MainView.interactable = false;
            await ActivityContainer.Find(ContainerKey.OverlayActivity).HideAsync(nameof(ActivityConnectionError));
            OnClickButtonTryAgainCallback?.Invoke();
        }
        public async UniTask OnClickButtonClose()
        {
            View.MainView.interactable = false;
            await ActivityContainer.Find(ContainerKey.OverlayActivity).HideAsync(nameof(ActivityConnectionError));
        }
        
    }
}