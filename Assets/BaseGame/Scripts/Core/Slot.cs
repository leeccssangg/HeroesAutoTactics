using System;
using Cysharp.Threading.Tasks;
using Game.Manager;
using LitMotion;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Core
{
    public class Slot : Holder
    {
        private Camera MainCamera { get; set; }
        protected InputManager InputManager { get; private set; }
        protected InGameHandleManager InGameHandleManager { get; private set; }
        protected FactoryManager FactoryManager { get; private set; }
        protected AbilityResolveManager AbilityResolveManager { get; private set; }

        [field: Title(nameof(Slot))]
        [field: SerializeField]
        public Transform FocusGroup { get; private set; }

        [field: SerializeField] public AnimationCurve FocusCurve { get; private set; }

        private MotionHandle FocusMotionHandle { get; set; }
        

        public Slot Construct(InputManager inputManager, InGameHandleManager inGameHandleManager, FactoryManager factoryManager, AbilityResolveManager abilityResolveManager)
        {
            MainCamera = inputManager.MainCamera;
            InputManager = inputManager;
            InGameHandleManager = inGameHandleManager;
            FactoryManager = factoryManager;
            AbilityResolveManager = abilityResolveManager;
            return this;
        }

        public void SetImageFocus(bool isFocus)
        {
            FocusMotionHandle.TryCancel();
            FocusGroup.gameObject.SetActive(isFocus);
            if (isFocus)
            {
                FocusMotionHandle = LMotion.Create(0f, 1f, 0.2f)
                    .WithEase(Ease.Linear)
                    .Bind(FocusMotionUpdateCallback);
            }
            return;
            void FocusMotionUpdateCallback(float value)
            {
                FocusGroup.localScale = Vector3.one * FocusCurve.Evaluate(value);
            }
        }

        #region Interact Functions

        public override void OnPointerDown(PointerEventData eventData)
        {

        }

        public override void OnPointerUp(PointerEventData eventData)
        {
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            if (InputManager.IsBlockPlayerInput) return;
            if (!CanDragIn) return;
            InputManager.SetCurrentHolderOverride(this);
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
            if (InputManager.IsBlockPlayerInput) return;
            if (InputManager.CurrentHolderOverride != this) return;
            InputManager.SetCurrentHolderOverride(null);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (InputManager.IsBlockPlayerInput) return;
            if (!HasHero) return;
            InputManager.SetCurrentHolderOverride(null);
            InputManager.SetCurrentHolderDrag(this);
            InputManager.CurrentDragHero = OwnerHero;
            InputManager.CurrentDragHero.StartDragUpdate().Forget();
            RemoveHero();
        }

        public override void OnDrag(PointerEventData eventData)
        {
            if (InputManager.IsBlockPlayerInput) return;
            if (!InputManager.CurrentDragHero) return;
            Vector2 newPosition = MainCamera.ScreenToWorldPoint(eventData.position);
            InputManager.CurrentDragHero.SetHeroTargetPosition(newPosition);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            if (InputManager.IsBlockPlayerInput) return;
            if (!InputManager.CurrentDragHero) return;

            if (!InputManager.CurrentHolderOverride
                || InputManager.CurrentHolderOverride == InputManager.CurrentHolderDrag)
            {
                OnForceBackToStartPosition();
            }

            if (InputManager.CurrentHolderOverride &&
                InputManager.CurrentHolderDrag &&
                InputManager.CurrentHolderDrag != InputManager.CurrentHolderOverride)
            {
                OnDragInSuccess(InputManager.CurrentHolderDrag, InputManager.CurrentHolderOverride,
                    InputManager.CurrentDragHero);
            }

            InputManager.CurrentDragHero.StopDragUpdate().Forget();
            InputManager.CurrentDragHero = null;
            InputManager.SetCurrentHolderDrag(null);
        }

        protected virtual void OnForceBackToStartPosition()
        {
            (InputManager.CurrentHolderDrag as Slot)!.SetImageFocus(false);
            InputManager.CurrentHolderDrag.AddHero(InputManager.CurrentDragHero);
            InputManager.InActiveSelect();
        }

        protected virtual void OnDragInSuccess(Holder dragHolder, Holder dropHolder, Hero heroDragging)
        {
        }

        #endregion
    }
}