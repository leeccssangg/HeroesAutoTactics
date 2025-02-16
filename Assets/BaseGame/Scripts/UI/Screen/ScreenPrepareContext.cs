using System;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Manager;
using TW.UGUI.MVPPattern;
using UnityEngine;
using R3;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Modals;
using TW.UGUI.Core.Screens;
using TW.Utility.Extension;
using UnityEngine.UI;

[Serializable]
public class ScreenPrepareContext 
{
    public static class Events
    {
        public static Action<bool> SetSellGroupVisible { get; set; }
        public static Action<bool, Hero, int> ShowHeroInfo { get; set; }
    }
    
    [HideLabel]
    [Serializable]
    public class UIModel : IAModel
    {
        [field: Title(nameof(UIModel))]
        [field: SerializeField] public ReactiveValue<int> PlayerCoin { get; private set; }
        [field: SerializeField] public ReactiveValue<int> PlayerHeart { get; private set; }
        [field: SerializeField] public ReactiveValue<int> PlayerWin { get; private set; }
        [field: SerializeField] public ReactiveValue<int> PlayerRound { get; private set; }
        [field: SerializeField] public ReactiveValue<int> FreeRollCount {get; private set;}
        public UniTask Initialize(Memory<object> args)
        {   
            PlayerCoin = InGameHandleManager.Instance.Coin;
            PlayerHeart = InGameHandleManager.Instance.Heart;
            PlayerWin = InGameHandleManager.Instance.Win;
            PlayerRound = InGameHandleManager.Instance.Round;
            FreeRollCount = InGameHandleManager.Instance.FreeRollCount;
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;} 

        [field: SerializeField] public TextMeshProUGUI TextPlayerCoin {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextPlayerHeart {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextPlayerWin {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextPlayerRound {get; private set;}
        [field: SerializeField] public Button ButtonRoll {get; private set;}
        [field: SerializeField] public Button ButtonEndTurn {get; private set;}
        [field: SerializeField] public Button ButtonFreeze {get; private set;}
        [field: SerializeField] public SellGroup SellGroup {get; private set;}
        [field: SerializeField] public GameObject BlockInteractGroup {get; private set;}
        [field: SerializeField] public GameObject FreeRollGroup {get; private set;}
        [field: SerializeField] public StoreSlot[] StoreSlots {get; private set;}
        [field: SerializeField] public Image[] ImageStroke {get; private set;}
        [field: SerializeField] public Image[] ImageBg {get; private set;}
        [field: SerializeField] public GameObject HeroInfoGroup {get; private set;}
        [field: SerializeField] public HeroImageGraphic HeroImageGraphic { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TextHeroName { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TextPassive { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TextCoin { get; private set; }
        public UniTask Initialize(Memory<object> args)
        {
            BlockInteractGroup.SetActive(false);
            InGameHandleManager.Instance.InitStoreSlot(StoreSlots);
            return UniTask.CompletedTask;
        }
        public void SetSellGroupVisible(bool isVisible)
        {
            SellGroup.SetVisible(isVisible);
        }
        public void ShowHeroInfo(bool isShow, Hero hero, int coin)
        {
            HeroInfoGroup.SetActive(isShow);
            if (!isShow) return;
            TextHeroName.text = hero.HeroConfigData.HeroName;
            TextPassive.text = hero.HeroConfigData.GetDescription(hero.GetStar());
            TextCoin.text = coin.ToString();
            HeroImageGraphic.Init(hero.HeroConfigData);
            
            Rarity rarity = hero.HeroConfigData.Rarity;
            RarityConfig rarityConfig = RarityGlobalConfig.Instance.GetRarityConfig(rarity);
            foreach (Image image in ImageStroke)
            {
                image.sprite = rarityConfig.RarityStroke;
            }
            foreach (Image image in ImageBg)
            {
                image.sprite = rarityConfig.RarityBackground;
            }
        }
        public void OnCoinChanged(int coin)
        {
            TextPlayerCoin.text = coin.ToString();
        }
        public void OnHeartChanged(int heart)
        {
            TextPlayerHeart.text = heart.ToString();
        }
        public void OnWinChanged(int win)
        {
            TextPlayerWin.text = $"{win.ToString()}/10";
        }
        public void OnRoundChanged(int turn)
        {
            TextPlayerRound.text = turn.ToString();
        }
        public void OnFreeRollCountChanged(int count)
        {
            FreeRollGroup.SetActive(count > 0);
        }
    }

    [HideLabel]
    [Serializable]
    public class UIPresenter : IAPresenter, IScreenLifecycleEventSimple
    {
        [field: SerializeField] public UIModel Model {get; private set;} = new();
        [field: SerializeField] public UIView View { get; set; } = new();        

        public async UniTask Initialize(Memory<object> args)
        {
            await Model.Initialize(args);
            await View.Initialize(args);
            
            Events.SetSellGroupVisible = View.SetSellGroupVisible;
            Events.ShowHeroInfo = View.ShowHeroInfo;

            View.ButtonRoll.SetOnClickDestination(OnButtonRollClicked);
            View.ButtonEndTurn.SetOnClickDestination(OnButtonEndTurnClicked);
            
            Model.PlayerCoin.ReactiveProperty.Subscribe(View.OnCoinChanged).AddTo(View.MainView);
            Model.PlayerHeart.ReactiveProperty.Subscribe(View.OnHeartChanged).AddTo(View.MainView);
            Model.PlayerWin.ReactiveProperty.Subscribe(View.OnWinChanged).AddTo(View.MainView);
            Model.PlayerRound.ReactiveProperty.Subscribe(View.OnRoundChanged).AddTo(View.MainView);
            Model.FreeRollCount.ReactiveProperty.Subscribe(View.OnFreeRollCountChanged).AddTo(View.MainView);
            
            InGameHandleManager.Instance.ReRollStoreSlots();
        }

        public UniTask Cleanup(Memory<object> args)
        {
            Events.SetSellGroupVisible = null;
            Events.ShowHeroInfo = null;
            return UniTask.CompletedTask;
        }
        private async UniTask OnButtonEndTurnClicked()
        {
            if (InputManager.Instance.IsBlockPlayerInput) return;

            View.BlockInteractGroup.SetActive(true);
            await InGameHandleManager.Instance.TryEndPrepareTurn();
            View.BlockInteractGroup.SetActive(false);
        }
        private void OnButtonRollClicked(Unit unit)
        {
            if (InputManager.Instance.IsBlockPlayerInput) return;
            if (Model.FreeRollCount.Value > 0)
            {
                InGameHandleManager.Instance.ConsumedFreeRollCount(1);
                InGameHandleManager.Instance.ReRollStoreSlots();
            }
            else if (Model.PlayerCoin.Value > 0)
            {
                InGameHandleManager.Instance.ConsumedCoin(1);
                InGameHandleManager.Instance.ReRollStoreSlots();
            }

        }


    }
}