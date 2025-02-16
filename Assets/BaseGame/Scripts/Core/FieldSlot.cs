using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Manager;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Core
{
    public class FieldSlot : Slot
    {
        [field: Title(nameof(FieldSlot))]
        [field: SerializeField]
        public FieldSlot[] FieldSlotAround { get; private set; }

        private static CancellationTokenSource EmptyFieldCancellationTokenSource { get; set; }

        private async UniTask WaitAndTryEmptyFieldSlot()
        {
            EmptyFieldCancellationTokenSource?.Cancel();
            EmptyFieldCancellationTokenSource?.Dispose();
            EmptyFieldCancellationTokenSource = new CancellationTokenSource();
            await UniTask.Delay(300, cancellationToken: EmptyFieldCancellationTokenSource.Token);
            InGameHandleManager.TryEmptyFieldSlot(this);
        }

        private void StopWaitAndTryEmptyFieldSlot()
        {
            EmptyFieldCancellationTokenSource?.Cancel();
            EmptyFieldCancellationTokenSource?.Dispose();
            EmptyFieldCancellationTokenSource = null;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            if (InputManager.IsBlockPlayerInput) return;
            if (!HasHero) return;
            InputManager.SetSelectedSlot(this);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);
            if (InputManager.IsBlockPlayerInput) return;
            StopWaitAndTryEmptyFieldSlot();
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);
            if (InputManager.IsBlockPlayerInput) return;
            if (!InputManager.CurrentDragHero) return;
            InputManager.SetSelectedSlot(this);
            if (!HasHero) return;
            if (OwnerHero.HeroConfigData != InputManager.CurrentDragHero.HeroConfigData ||
                OwnerHero.IsMaxStar() ||
                InputManager.CurrentDragHero.IsMaxStar())
            {
                WaitAndTryEmptyFieldSlot().Forget();
            }
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
            if (InputManager.IsBlockPlayerInput) return;
            StopWaitAndTryEmptyFieldSlot();
            if (!InputManager.CurrentDragHero) return;
            SetImageFocus(false);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
            if (InputManager.IsBlockPlayerInput) return;
            if (!InputManager.CurrentDragHero) return;
            InputManager.CurrentDragHero.SetHeroStatGroup(false);
            InputManager.CurrentDragHero.SetHeroStarProcess(false);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (InputManager.IsBlockPlayerInput) return;
            if (InputManager.CurrentDragHero)
            {
                InputManager.CurrentDragHero.SetHeroStatGroup(true);
                InputManager.CurrentDragHero.SetHeroStarProcess(true);
            }

            base.OnEndDrag(eventData);
        }

        protected override void OnDragInSuccess(Holder dragHolder, Holder dropHolder, Hero heroDragging)
        {
            if (CanFusion(dragHolder, dropHolder, heroDragging))
            {
                Fusion(dragHolder, dropHolder, heroDragging).Forget();
            }
            else if (CanMove(dragHolder, dropHolder, heroDragging))
            {
                Move(dragHolder, dropHolder, heroDragging);
            }
            else if (CanSell(dragHolder, dropHolder, heroDragging))
            {
                Sell(dragHolder, dropHolder, heroDragging).Forget();
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

        protected override void OnForceBackToStartPosition()
        {
            InputManager.CurrentDragHero.SetHeroStarProcess(true);
            base.OnForceBackToStartPosition();
        }

        private bool CanFusion(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            if (toHolder is SellGroup) return false;
            if (!toHolder.HasHero) return false;
            if (heroDragging.IsMaxStar()) return false;
            if (toHolder.OwnerHero.IsMaxStar()) return false;
            return toHolder.OwnerHero.HeroConfigData == heroDragging.HeroConfigData;
        }

        private async UniTask Fusion(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            InputManager.SetBlockPlayerInput(true);
            InputManager.InActiveSelect();
            AbilityResolveManager.SetBlockResolve(true);
            
            int oldStar = toHolder.OwnerHero.GetStar();
            
            toHolder.AddHeroImmediate(heroDragging);
            fromHolder.RemoveHero();
            heroDragging.SetVisible(false);
            int newStar = toHolder.OwnerHero.GetStar();
            if (oldStar < newStar)
            {
                toHolder.OwnerHero.TryTriggerAbility<SelfStarUpTrigger>(TickRate.Normal, this.GetCancellationTokenOnDestroy());
                InGameHandleManager.TryTriggerAllyAbility<AllyStarUpTrigger>(toHolder.OwnerHero, TickRate.Normal, this.GetCancellationTokenOnDestroy());
                InGameHandleManager.TryTriggerAllyAbility<EveryoneInTeamStarUpTrigger>(toHolder.OwnerHero, TickRate.Normal, this.GetCancellationTokenOnDestroy());
            }

            if (fromHolder is StoreSlot { IsFreeze: true } shopSlot)
            {
                shopSlot.ToggleFreeze();
            }
            
            AbilityResolveManager.SetBlockResolve(false);
            await UniTask.WaitUntil(AbilityResolveManager.IsResolvingCompleteFunc, cancellationToken: this.GetCancellationTokenOnDestroy());
            heroDragging.SelfDespawnImmediate();
            InputManager.SetBlockPlayerInput(false);
        }

        private bool CanMove(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            if (toHolder is SellGroup) return false;
            return !toHolder.HasHero;
        }

        private void Move(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            toHolder.AddHeroImmediate(heroDragging);

            fromHolder.RemoveHero();

            InputManager.InActiveSelect();
            if (fromHolder is StoreSlot { IsFreeze: true } shopSlot)
            {
                shopSlot.ToggleFreeze();
            }
        }

        private bool CanSell(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            return toHolder is SellGroup;
        }

        private async UniTask Sell(Holder fromHolder, Holder toHolder, Hero heroDragging)
        {
            InputManager.SetBlockPlayerInput(true);
            InputManager.InActiveSelect();
            AbilityResolveManager.SetBlockResolve(true);


            InGameHandleManager.AddCoin(heroDragging.GetCost());
            heroDragging.TryTriggerAbility<SelfSellTrigger>(TickRate.Normal, this.GetCancellationTokenOnDestroy());
            fromHolder.RemoveHero();
            heroDragging.SetVisible(false);
            AbilityResolveManager.SetBlockResolve(false);
            await UniTask.WaitUntil(AbilityResolveManager.IsResolvingCompleteFunc, cancellationToken: this.GetCancellationTokenOnDestroy());

            heroDragging.SelfDespawnImmediate();
            InputManager.SetBlockPlayerInput(false);
        }

        public void ClearFieldSlot()
        {
            OwnerHero?.SelfDespawnImmediate();
            RemoveHero();
        }

        public void LoadFieldSlot(InGameHeroData inGameHeroData)
        {
            if (inGameHeroData.HeroId == -1) return;

            InitNewHeroImmediate(FactoryManager.CreateHero(inGameHeroData.HeroId)
                .SetHeroPiece(inGameHeroData.Pieces)
                .InitBaseStat()
                .InitStat(inGameHeroData));
        }
    }
}