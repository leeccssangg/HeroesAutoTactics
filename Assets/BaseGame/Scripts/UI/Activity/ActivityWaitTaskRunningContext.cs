using System;
using Cysharp.Threading.Tasks;
using LitMotion;
using TW.UGUI.MVPPattern;
using UnityEngine;
using Sirenix.OdinInspector;
using TW.UGUI.Core.Activities;

[Serializable]
public class ActivityWaitTaskRunningContext
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
        [field: SerializeField]
        public CanvasGroup MainView { get; private set; }
        [field: SerializeField]
        public CanvasGroup RootView  { get; private set; }
        public UniTask Initialize(Memory<object> args)
        {
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IActivityLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model { get; private set; } = new();
        [field: SerializeField] public UIView View { get; set; } = new();
        private MotionHandle MotionHandle { get; set; }
        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            View.RootView.alpha = 0;
            MotionHandle.TryCancel();
            MotionHandle = LMotion.Create(0f, 1f, 0.2f)
                .WithDelay(0.2f)
                .Bind(value => View.RootView.alpha = value);
        }

        public UniTask Cleanup(Memory<object> args)
        {
            MotionHandle.TryCancel();
            return UniTask.CompletedTask;
        }
    }
}