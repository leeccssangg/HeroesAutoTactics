using System;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Manager;
using R3;
using Sirenix.OdinInspector;
using Spine.Unity;
using TMPro;
using TW.Utility.Extension;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Zenject;

namespace Game.Core
{
    public class StoreSlot : Slot
    {

        [field: Title(nameof(StoreSlot))]
        [field: SerializeField] public Image[] ImageStroke {get; private set;}
        [field: SerializeField] public Image[] ImageBg {get; private set;}
        [field: SerializeField] public bool IsFreeze { get; private set; }
        [field: SerializeField] private GameObject FreezeGroup { get; set; }
        [field: SerializeField] public HeroImageGraphic HeroImageGraphic {get; private set;}

        [field: SerializeField] public GameObject StatGroup {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextHealthPoint {get; private set;}
        [field: SerializeField] public TextMeshProUGUI TextAttackDamage {get; private set;}
        private IDisposable HealthPointDisposable { get; set; }
        private IDisposable AttackDamageDisposable { get; set; }
        public void ReRollHeroFromPool()
        {
            if (IsFreeze) return;
            if (OwnerHero != null)
            {
                Destroy(OwnerHero.gameObject);
            }

            if (InputManager.SelectedSlot == this)
            {
                InputManager.InActiveSelect();
            }
            InitNewHeroImmediate(FactoryManager.CreateRandomHero(InGameHandleManager.Round).InitBaseStat()
                .SetHeroInstancePosition(Transform.position)
                .SetHeroStarProcess(false)
                .SetHeroStatGroup(false)
                .SetVisible(false)
                .SetHolder(this));
            Rarity rarity = OwnerHero.HeroConfigData.Rarity;
            RarityConfig rarityConfig = RarityGlobalConfig.Instance.GetRarityConfig(rarity);
            foreach (Image image in ImageStroke)
            {
                image.sprite = rarityConfig.RarityStroke;
            }
            foreach (Image image in ImageBg)
            {
                image.sprite = rarityConfig.RarityBackground;
            }
            HeroImageGraphic.Init(OwnerHero.HeroConfigData);
            HeroImageGraphic.SetActive(true);
            HeroImageGraphic.SetColor(Color.white);
            StatGroup.SetActive(true);
            
            HealthPointDisposable?.Dispose();
            AttackDamageDisposable?.Dispose();
            HealthPointDisposable = OwnerHero.HealthPoint.ReactiveProperty.Subscribe(OnHealthPointChanged).AddTo(this);
            AttackDamageDisposable = OwnerHero.AttackDamage.ReactiveProperty.Subscribe(OnAttackDamageChanged).AddTo(this);
            
        }
        private void OnHealthPointChanged(int value)
        {
            TextHealthPoint.text = value.ToString();
        }
        private void OnAttackDamageChanged(int value)
        {
            TextAttackDamage.text = value.ToString();
        }

        private void SetImageFreeze(bool isFreeze)
        {
            FreezeGroup.SetActive(isFreeze);
        }

        public void ToggleFreeze()
        {
            IsFreeze = !IsFreeze;
            SetImageFreeze(IsFreeze);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            if (!HasHero) return;
            InputManager.SetSelectedSlot(this);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (!InputManager.CurrentDragHero) return;
            InputManager.CurrentDragHero.SetVisible(true);
            HeroImageGraphic.SetColor(Color.black);

        }
        
        protected override void OnDragInSuccess(Holder dragHolder, Holder dropHolder, Hero heroDragging)
        {
            if (CanFusion(dragHolder, dropHolder, heroDragging))
            {
                // merge success
                Fusion(dragHolder, dropHolder, heroDragging).Forget();
            }
            else if (CanMove(dragHolder, dropHolder, heroDragging))
            {
                //move success
                Move(dragHolder, dropHolder, heroDragging).Forget();
            }
            else if (TryFreeze(dragHolder, dropHolder, heroDragging))
            {
                //freeze success
            }
            else
            {
                OnForceBackToStartPosition();
            }

            if (dragHolder is Slot dragSlot)
            {
                dragSlot.SetImageFocus(false);
            }

            if (dropHolder is Slot dropSlot)
            {
                dropSlot.SetImageFocus(false);
            }
        }
        private bool CanFusion(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            if (!InGameHandleManager.IsEnoughCoin(3)) return false;
            if (toHolder is FreezeButton) return false;
            if (!toHolder.HasHero) return false;
            if (heroDragging.IsMaxStar()) return false;
            if (toHolder.OwnerHero.IsMaxStar()) return false;
            if (toHolder.OwnerHero.HeroConfigData != heroDragging.HeroConfigData) return false;
            return true;
        }
        private async UniTask Fusion(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            InputManager.SetBlockPlayerInput(true);
            InputManager.InActiveSelect();
            AbilityResolveManager.SetBlockResolve(true);
            InGameHandleManager.ConsumedCoin(3);

            HeroImageGraphic.SetActive(false);
            StatGroup.SetActive(false);
            
            
            toHolder.AddHeroImmediate(heroDragging);
            toHolder.OwnerHero.SetHeroStarProcess(true);
            toHolder.OwnerHero.SetHeroStatGroup(true);
            toHolder.OwnerHero.TryTriggerAbility<SelfBuyTrigger>(TickRate.Normal, this.GetCancellationTokenOnDestroy());
            fromHolder.RemoveHero();
            heroDragging.SetVisible(false);
            
            if (fromHolder is StoreSlot { IsFreeze: true } storeSlot)
            {
                storeSlot.ToggleFreeze();
            }
            
            AbilityResolveManager.SetBlockResolve(false);
            await UniTask.WaitUntil(AbilityResolveManager.IsResolvingCompleteFunc, cancellationToken: this.GetCancellationTokenOnDestroy());
            // heroDragging.SelfDespawnImmediate();
            InputManager.SetBlockPlayerInput(false);
        }
        private bool CanMove(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            if (!InGameHandleManager.IsEnoughCoin(3)) return false;
            if (toHolder is FreezeButton) return false;
            if (toHolder.HasHero) return false;
            return true;
        }
        
        private async UniTask Move(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            InputManager.SetBlockPlayerInput(true);
            InputManager.InActiveSelect();
            AbilityResolveManager.SetBlockResolve(true);
            InGameHandleManager.ConsumedCoin(3);
            HeroImageGraphic.SetActive(false);
            StatGroup.SetActive(false);
            
            
            toHolder.AddHeroImmediate(heroDragging);
            toHolder.OwnerHero.SetHeroStarProcess(true);
            toHolder.OwnerHero.SetHeroStatGroup(true);
            toHolder.OwnerHero.TryTriggerAbility<SelfBuyTrigger>(TickRate.Normal, this.GetCancellationTokenOnDestroy());
            fromHolder.RemoveHero();
            if (fromHolder is StoreSlot { IsFreeze: true } storeSlot)
            {
                storeSlot.ToggleFreeze();
            }

            AbilityResolveManager.SetBlockResolve(false);
            await UniTask.WaitUntil(AbilityResolveManager.IsResolvingCompleteFunc, cancellationToken: this.GetCancellationTokenOnDestroy());
            InputManager.SetBlockPlayerInput(false);
        }

        private bool TryFreeze(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            if (fromHolder is not StoreSlot shopSlot) return false;
            if (toHolder is not FreezeButton) return false;
            fromHolder.AddHeroImmediate(heroDragging);
            shopSlot.ToggleFreeze();
            return true;
        }
        
        protected override void OnForceBackToStartPosition()
        {
            base.OnForceBackToStartPosition();
            InputManager.CurrentDragHero.SetVisible(false);
            HeroImageGraphic.SetColor(Color.white);
        }
        public void ClearStoreSlot()
        {
            OwnerHero?.SelfDespawnImmediate();
            RemoveHero();
        }
    }
    
}