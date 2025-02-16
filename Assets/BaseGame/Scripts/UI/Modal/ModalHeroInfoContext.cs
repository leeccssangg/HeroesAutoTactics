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

[Serializable]
public class ModalHeroInfoContext 
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
        [field: SerializeField] public int HeroId { get; private set; }
        [field: SerializeField] public HeroConfigData HeroConfigData { get; private set; }
        [field: SerializeField] public EachHeroUpgradeData HeroUpgradeData { get; private set; }

        public UniTask Initialize(Memory<object> args)
        {   
            HeroId = (int)args.Span[0];
            HeroConfigData = HeroManager.Instance.GetHeroConfigData(HeroId);
            HeroUpgradeData = HeroManager.Instance.GetHeroUpgradeData(HeroId);
            return UniTask.CompletedTask;
        }
    }
    
    [HideLabel]
    [Serializable]
    public class UIView : IAView
    {
        [field: Title(nameof(UIView))]
        [field: SerializeField] public CanvasGroup MainView {get; private set;}
        [field: SerializeField] public Image ImgIcon {get; private set;}
        [field: SerializeField] public Image ImgFamilyBGTop {get; private set;}
        [field: SerializeField] public Image ImgFamilyBGMid { get; private set; }
        [field: SerializeField] public Image ImgFamilyIconTop { get; private set; }
        [field: SerializeField] public Image ImgFamilyIconMid { get; private set; }
        [field: SerializeField] public Image ImgRarity {get; private set;}

        [field: SerializeField] public TextMeshProUGUI TxtHeroName {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TxtHeroLevel {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TxtPiece { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtHeroRarity { get; private set; }
        [field: SerializeField] public TextMeshProUGUI TxtHeroFamily {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TxtAttackDamage {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TxtHealthPoint {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TxtResourceUpgrade { get; private set; }

        [field: SerializeField] public Slider SliderPiece { get; private set; }

        [field: SerializeField] public List<UIHeroAbilityInventoryInfo> ListUIHeroAbilityInventoryInfo { get; private set;}
        [field: SerializeField] public List<UIHeroUpgradeInventoryInfo> ListUIHeroUpgradeInventoryInfo { get; private set; }

        [field: SerializeField] public Button BtnUpgrade { get; private set; }
        [field: SerializeField] public Button BtnLock { get; private set; }
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

            SetupHero();
            SetupAbilityHero();
            SetupUpgradeHero();

            Model.HeroUpgradeData.Level.ReactiveProperty.Subscribe(OnLevelHeroChange).AddTo(View.MainView);
            Model.HeroUpgradeData.Piece.ReactiveProperty.Subscribe(OnPieceHeroChange).AddTo(View.MainView);
            //TODO Add subscribe resource

            View.BtnUpgrade.SetOnClickDestination(OnClickBtnUpgrade);
            View.BtnLock.SetOnClickDestination(OnClickBtnLock);
            View.BtnClose.SetOnClickDestination(OnClickBtnClose);
        }
        private void SetupHero()
        {
            View.ImgIcon.sprite = Model.HeroConfigData.SpriteIcon;
            //View.ImgFamilyBGTop.color = Model.HeroConfigData.HeroFamily.GetColor();
            //View.ImgFamilyBGMid.color = Model.HeroConfigData.HeroFamily.GetColor();
            //View.ImgFamilyIconTop.sprite = Model.HeroConfigData.HeroFamily.GetIcon();
            //View.ImgFamilyIconMid.sprite = Model.HeroConfigData.HeroFamily.GetIcon();
            //View.ImgRarity.sprite = Model.HeroConfigData.Rarity.GetIcon();

            View.TxtHeroName.text = $"{Model.HeroConfigData.HeroName}";
            //View.TxtHeroLevel.text = $"Lv.{Model.HeroUpgradeData.Level}";
            //View.TxtPiece.text = $"{Model.HeroUpgradeData.Piece}/{HeroManager.Instance.GetPieceNeededUpgradeHero(Model.HeroConfigData.HeroId)}";
            View.TxtHeroRarity.text = $"{Model.HeroConfigData.Rarity.ToString()}";
            View.TxtHeroFamily.text = $"{Model.HeroConfigData.HeroFamily.ToString()}";
            //View.TxtAttackDamage.text = $"{Model.HeroConfigData.AttackDamage.ToString()}";
            //View.TxtHealthPoint.text = $"{Model.HeroConfigData.HealthPoint.ToString()}";

            //View.SliderPiece.value = HeroManager.Instance.GetHeroPieceProcess(Model.HeroConfigData.HeroId);
        }
        private void SetupAbilityHero()
        {
            for (int i = 0; i < Model.HeroConfigData.AbilityPower.Length; i++)
            {
                View.ListUIHeroAbilityInventoryInfo[i].Setup(Model.HeroConfigData.AbilityPower[i]);
            }
        }
        private void SetupUpgradeHero()
        {
            //for (int i = 0; i < Model.HeroConfigData.AbilityPower.Length; i++)
            //{
            //    View.ListUIHeroUpgradeInventoryInfo[i].Setup(Model.HeroConfigData.AbilityPower[i]);
            //}
        }
        private void OnLevelHeroChange(int level)
        {
            if(level >= 1)
            {
                View.BtnUpgrade.gameObject.SetActive(true);
                View.BtnLock.gameObject.SetActive(false);
            }
            else
            {
                View.BtnUpgrade.gameObject.SetActive(false);
                View.BtnLock.gameObject.SetActive(true);
            }
            View.TxtHeroLevel.text = $"Lv.{level}";
            View.TxtAttackDamage.text = $"{Model.HeroConfigData.AttackDamage.ToString()}";
            View.TxtHealthPoint.text = $"{Model.HeroConfigData.HealthPoint.ToString()}";
            //TODO Add resource upgrade text
        }
        private void OnPieceHeroChange(int piece)
        {
            
            if(Model.HeroUpgradeData.Level >= 1)
            {
                View.BtnUpgrade.interactable = HeroManager.Instance.IsUpgradeAbleHero(Model.HeroConfigData.HeroId);
                View.SliderPiece.value = HeroManager.Instance.GetHeroPieceProcess(Model.HeroConfigData.HeroId);
                View.TxtPiece.text = $"{piece}/{HeroManager.Instance.GetPieceNeededUpgradeHero(Model.HeroConfigData.HeroId)}";
            }
            else
            {
                View.BtnUpgrade.interactable = false;
                View.SliderPiece.value = 0;
                View.TxtPiece.text = $"0/0";
            }
            
        }
        private void OnResourceUpgradeChange(int resource)
        {
            //View.TxtResource.text = $"{resource}";
            if (Model.HeroUpgradeData.Level >= 1)
            {
                View.BtnUpgrade.interactable = HeroManager.Instance.IsUpgradeAbleHero(Model.HeroConfigData.HeroId);
            }
            else
            {
                View.BtnUpgrade.interactable = false;
            }
        }
        private void OnClickBtnUpgrade(Unit _)
        {
            HeroManager.Instance.UpgradeHero(Model.HeroConfigData.HeroId);
        }
        private void OnClickBtnLock(Unit _)
        {
            //HeroManager.Instance.LockHero(Model.HeroConfigData.HeroId);
        }
        private void OnClickBtnClose(Unit _)
        {
            ModalContainer.Find(ContainerKey.Modals).Pop(true);
        }
    }
}