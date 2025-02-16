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

[Serializable]
public class ScreenMainMenuTabContext 
{
    public enum MainMenuTab
    {
        None = -1,
        Shop,
        Hero,
        MainMenu,
    }
    public static class Events
    {
        public static Action<bool> ForceShowTab { get; set; }
        public static Action<bool> InteractableCanvasGroup { get; set; }
        public static Action<bool> NotiQuest { get; set; }
        public static Action<bool> NotiInventory { get; set; }
        public static Action<bool> NotiMainMenu { get; set; }
        public static Action<Vector3, int> DelayIncreaseValueGem { get; set; }
    }
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
        [field: SerializeField] public CanvasGroup MainView { get; private set; }
        //[field: SerializeField] public UIResourceStar UIResourceStar { get; private set; }
        //[field: SerializeField] public UIResourceGem UIResourceGem { get; private set; }
        //[field: SerializeField] public UIResourceMoney UIResourceMoney { get; private set; }
        [field: SerializeField] public TabGroupButton TabGroupButton { get; private set; }
        [field: SerializeField] public SheetMainMenuContainer SheetMainMenuContainer { get; private set; }
        [field: SerializeField] public FeelAnimation ShowTabAnimation { get; private set; }
        [field: SerializeField] public FeelAnimation HideTabAnimation { get; private set; }
        [field: SerializeField] public Button ButtonAvatar { get; private set; }
        [field: SerializeField] public Button ButtonSettings { get; private set; }
        [field: SerializeField] public GameObject GONotiQuest { get; private set; }
        [field: SerializeField] public GameObject GONotiShop { get; private set; }
        [field: SerializeField] public GameObject GONotiInventoryEquipment { get; private set; }
        [field: SerializeField] public GameObject GONotiInventoryUpgrade { get; private set; }
        [field: SerializeField] public GameObject GONotiMain { get; private set; }
        public UniTask Initialize(Memory<object> args)
        {
            //UIResourceStar.Initialize(args);
            //UIResourceGem.Initialize(args);
            //UIResourceMoney.Initialize(args);
            return UniTask.CompletedTask;
        }

    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model { get; private set; }
        [field: SerializeField] public UIView View { get; set; } = new();
        private int[] SheetId { get; set; } = new int[4];
        [field: SerializeField] public MainMenuTab CurrentMainMenuTab { get; private set; } = MainMenuTab.None;

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            //View.ButtonAvatar.SetOnClickDestination(OnClickButtonAvatar);
            //View.ButtonSettings.SetOnClickDestination(OnClickButtonSettings);

            SheetOptions options0 = new SheetOptions(nameof(SheetShop), OnSheetLoaded);
            SheetId[0] = await View.SheetMainMenuContainer.RegisterAsync(options0, args);

            SheetOptions options1 = new SheetOptions(nameof(SheetHero), OnSheetLoaded);
            SheetId[1] = await View.SheetMainMenuContainer.RegisterAsync(options1, args);

            SheetOptions option2 = new SheetOptions(nameof(SheetMainMenu), OnSheetLoaded);
            SheetId[2] = await View.SheetMainMenuContainer.RegisterAsync(option2, args);

            //SheetOptions option3 = new SheetOptions(nameof(SheetShop), OnSheetLoaded);
            //SheetId[3] = await View.SheetMainMenuContainer.RegisterAsync(option3, args);

            View.TabGroupButton.Setup<MainMenuTab>(OnOpenTab);
            Events.ForceShowTab = OnForceShowTab;
            Events.InteractableCanvasGroup = OnPlayGame;
            //Events.NotiQuest = OnNotiQuest;
            //Events.DelayIncreaseValueGem = OnDelayAddGem;
            //InventoryManager.Events.OnInventoryChanged += OnInventoryChanged;

            //PlayerResource.Get(GameResource.Type.Money).ReactiveProperty.Subscribe(_ => OnInventoryChanged()).AddTo(View.MainView);
            //PlayerProfileData.Instance.WinCount.ReactiveProperty
            //    .CombineLatest(PlayerProfileData.Instance.WinRewardCollected.ReactiveProperty, (wc, wr) => (wc, wr))
            //    .Subscribe(_ => OnNotiMainMenu())
            //    .AddTo(View.MainView);
            //OnNotiQuest(AllQuestManager.Instance.IsShowNotiDailyQuest());
        }

        public UniTask Cleanup(Memory<object> args)
        {
            Events.ForceShowTab = null;
            Events.InteractableCanvasGroup = null;
            Events.NotiQuest = null;
            Events.DelayIncreaseValueGem = null;
            //InventoryManager.Events.OnInventoryChanged -= OnInventoryChanged;
            return UniTask.CompletedTask;
        }

        public void DidPushEnter(Memory<object> args)
        {
            //ShowSheet(1).Forget();
            OpenTab(MainMenuTab.MainMenu);
        }

        private void OnSheetLoaded(int sheetId, Sheet sheet, Memory<object> args)
        {
            (sheet as ISetupAble)?.Setup();
        }
        private void OnOpenTab(MainMenuTab tab)
        {
            if (CurrentMainMenuTab == tab) return;
            //Debug.Log("OnOpenTab: " + tab);
            CloseAllPanel();
            switch (tab)
            {
                case MainMenuTab.MainMenu:
                    ShowSheet(2).Forget();
                    //Debug.Log("MainMenu");
                    //OpenTab(MainMenuTab.MainMenu);
                    break;
                case MainMenuTab.Hero:
                    ShowSheet(1).Forget();
                    //OpenTab(MainMenuTab.Weapon);
                    break;
                case MainMenuTab.Shop:
                    ShowSheet(0).Forget();
                    //OpenTab(MainMenuTab.Shop);
                    break;
                default:
                    break;
            }
            CurrentMainMenuTab = tab;
        }
        private async UniTask ShowSheet(int index)
        {
            if (View.SheetMainMenuContainer.IsInTransition)
            {
                return;
            }

            if (View.SheetMainMenuContainer.ActiveSheetId == SheetId[index])
            {
                // This sheet is already displayed.
                return;
            }
            //OpenTab((MainMenuTab)index);
            await View.SheetMainMenuContainer.ShowAsync(SheetId[index], true);
        }
        public void OpenTab(MainMenuTab tab)
        {
            if (tab == MainMenuTab.None)
            {
                if (CurrentMainMenuTab == MainMenuTab.None)
                {
                    OnClickTabButton(MainMenuTab.MainMenu);
                }
                else
                {
                    tab = CurrentMainMenuTab;
                    CurrentMainMenuTab = MainMenuTab.None;
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
        private void OnClickTabButton(MainMenuTab tab)
        {
            View.TabGroupButton.OnClickButton(tab);
        }
        private void CloseAllPanel()
        {

        }
        private void OnForceShowTab(bool isShow)
        {
            if (isShow)
            {
                View.ShowTabAnimation.Play();
            }
            else
            {
                View.HideTabAnimation.Play();
            }
        }
        private void OnClickButtonAvatar(Unit _)
        {
            Debug.Log("OnClickButtonAvatar");
        }
        private void OnClickButtonSettings(Unit _)
        {
            //ViewOptions view = new ViewOptions(nameof(ModalSettings));
            //ModalContainer.Find(ContainerKey.Modals).PushAsync(view);
        }
        private void OnPlayGame(bool isTrue)
        {
            View.MainView.interactable = isTrue;
        }
        private void OnNotiQuest(bool isTrue)
        {
            //View.GONotiQuest.gameObject.SetActive(isTrue);
            //View.GONotiQuest.PlayAnimation(isTrue);
        }
        private void OnNotiMainMenu()
        {
            //WinStage currentWinRewardStage = WinRewardGlobalConfig.Instance.GetCurrentWinStage();
            //View.GONotiMain.gameObject.SetActive(PlayerProfileData.Instance.WinCount >= currentWinRewardStage.WinCount);
            //View.GONotiMain.PlayAnimation(PlayerProfileData.Instance.WinCount >= currentWinRewardStage.WinCount);
        }
        private void OnInventoryChanged()
        {
            //View.GONotiInventoryEquipment.gameObject.SetActive(InventoryManager.Instance.IsHasAnyBetterEquipment());
            //View.GONotiInventoryEquipment.PlayAnimation(InventoryManager.Instance.IsHasAnyBetterEquipment());
            //View.GONotiInventoryUpgrade.gameObject.SetActive(InventoryManager.Instance.IsAnyEquipmentCanUpgrade() &&
                                                    //!InventoryManager.Instance.IsHasAnyBetterEquipment());
            //View.GONotiInventoryUpgrade.PlayAnimation(InventoryManager.Instance.IsAnyEquipmentCanUpgrade() &&
            //!InventoryManager.Instance.IsHasAnyBetterEquipment());
        }
        private void OnDelayAddGem(Vector3 position, int value)
        {
            //View.UIResourceGem.SetDelayIncreaseValue(position, value);
        }
    }
}