using System;
using Cysharp.Threading.Tasks;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;

[Serializable]
public class ActivityChangeViewContext 
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
        [field: SerializeField] public FeelAnimation OpenAnimation {get; private set;}
        [field: SerializeField] public FeelAnimation CloseAnimation {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {

            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        
        private Action OnClose { get; set; }
        private Action OnOpen { get; set; }
        private float Duration { get; set; }
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            OnClose= (Action)args.Span[0];
            OnOpen = (Action)args.Span[1];
            Duration = (float)args.Span[2];
        }

        public void DidEnter(Memory<object> args)
        {
            OnEnterComplete().Forget();
        }
        private async UniTask OnEnterComplete()
        {
            await View.CloseAnimation.PlayAsync();
            OnClose?.Invoke();
            await UniTask.Delay(TimeSpan.FromSeconds(Duration));
            await View.OpenAnimation.PlayAsync();
            OnOpen?.Invoke();
            await ActivityContainer.Find(ContainerKey.OverlayActivity).HideAsync(nameof(ActivityChangeView));
        }
    }
}