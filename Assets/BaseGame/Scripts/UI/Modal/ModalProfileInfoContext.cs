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
using static ScreenMainMenuTabContext;

[Serializable]
public class ModalProfileInfoContext 
{
    public enum TabType
    {
        None = -1,
        Avatar,
        Frame,
    }
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
        [field: SerializeField] public ReactiveValue<string> UserName { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            UserName = PlayerProfileManager.Instance.UserName;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}

        [field: SerializeField] public TabGroupButton TabGroupButton {get; private set;}
        [field: SerializeField] public SheetPlayerInfoContainer SheetPlayerInfoContainer {get; private set;}

        [field: SerializeField] public Button BtnClose {get; private set;}
        [field: SerializeField] public Button BtnChangeName {get; private set;}

        [field: SerializeField] public UIPlayerInfo UIPlayerInfo {get; private set;}
        
        public UniTask Initialize(Memory<object> args)
        {
            UIPlayerInfo.Initialize(args);
            return UniTask.CompletedTask;
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IModalLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();
        private int[] SheetId { get; set; } = new int[2];
        [field: SerializeField] public TabType CurrerntTab { get; private set; } = TabType.None;

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);

            SheetOptions options0 = new SheetOptions(nameof(SheetPlayerInfoAvatar), OnSheetLoaded);
            SheetId[0] = await View.SheetPlayerInfoContainer.RegisterAsync(options0, args);

            SheetOptions options1 = new SheetOptions(nameof(SheetPlayerInfoFrame), OnSheetLoaded);
            SheetId[1] = await View.SheetPlayerInfoContainer.RegisterAsync(options1, args);


            View.TabGroupButton.Setup<TabType>(OnOpenTab);
            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
            View.BtnChangeName.SetOnClickDestination(OnClickBtnChangeName);
        }
        public void DidPushEnter(Memory<object> args)
        {
            //ShowSheet(1).Forget();
            OpenTab(TabType.Avatar);
        }
        private void OnSheetLoaded(int sheetId, Sheet sheet, Memory<object> args)
        {
            (sheet as ISetupAble)?.Setup();
        }
        private void OnOpenTab(TabType tab)
        {
            if (CurrerntTab == tab) return;
            //Debug.Log("OnOpenTab: " + tab);
            //CloseAllPanel();
            switch (tab)
            {
                case TabType.Avatar:
                    ShowSheet(0).Forget();
                    //Debug.Log("MainMenu");
                    //OpenTab(MainMenuTab.MainMenu);
                    break;
                case TabType.Frame:
                    ShowSheet(1).Forget();
                    //OpenTab(MainMenuTab.Weapon);
                    break;
                default:
                    break;
            }
            CurrerntTab = tab;
        }
        private async UniTask ShowSheet(int index)
        {
            if (View.SheetPlayerInfoContainer.IsInTransition)
            {
                return;
            }

            if (View.SheetPlayerInfoContainer.ActiveSheetId == SheetId[index])
            {
                // This sheet is already displayed.
                return;
            }
            //OpenTab((MainMenuTab)index);
            await View.SheetPlayerInfoContainer.ShowAsync(SheetId[index], true);
        }
        public void OpenTab(TabType tab)
        {
            if (tab == TabType.None)
            {
                if (CurrerntTab == TabType.None)
                {
                    OnClickTabButton(TabType.Avatar);
                }
                else
                {
                    tab = CurrerntTab;
                    CurrerntTab = TabType.None;
                    OnClickTabButton(tab);
                }
            }
            else
            {
                OnClickTabButton(tab);
            }
            //OnClickTabButton(tab);
            //m_ContentTransform.GetComponent<RectTransform>().DOAnchorPosY(1040, 0.45f).SetEase(Ease.OutBack);
            // m_DoTweenAnimation.tween.Restart();
        }
        private void OnClickTabButton(TabType tab)
        {
            View.TabGroupButton.OnClickButton(tab);
        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }
        private void OnClickBtnChangeName(Unit _)
        {
            ViewOptions options = new ViewOptions(nameof(ModalChangeUserName));
            ModalContainer.Find(ContainerKey.Modals).Push(options);
        }
    }
}