using System;
using System.Collections;
using System.Collections.Generic;
using Game.Core;
using R3;
using R3.Triggers;
using TW.UGUI.Core.Activities;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;

namespace Game.Manager
{
    public class InputManager : Singleton<InputManager>
    {
        [Inject] public Camera MainCamera { get; private set; }
        [field: SerializeField] public bool IsBlockPlayerInput {get; private set;}
        [field: SerializeField] public Hero CurrentDragHero { get; set; }
        [field: SerializeField] public Holder CurrentHolderDrag { get; private set; }
        [field: SerializeField] public Holder CurrentHolderOverride { get; private set; }
        [field: SerializeField] public Slot SelectedSlot { get; private set; }
        // [field: SerializeField] public FreezeButton FreezeButton { get; private set; }

#if UNITY_EDITOR
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Break();
            }
        }
#endif
        public void SetBlockPlayerInput(bool isBlock)
        {
            IsBlockPlayerInput = isBlock;
        }
        public void SetSelectedSlot(Slot slot)
        {
            SelectedSlot?.SetImageFocus(false);
            if (SelectedSlot != null && SelectedSlot.HasHero)
            {
                SelectedSlot.OwnerHero.SetHeroOrder(100);
                SelectedSlot.OwnerHero.SetHeroScaleTarget(1);
            }

            SelectedSlot = slot;
            SelectedSlot.SetImageFocus(true);
            if (SelectedSlot.HasHero && CurrentDragHero == null)
            {
                SelectedSlot.OwnerHero.SetHeroOrder(200);
                SelectedSlot.OwnerHero.SetHeroScaleTarget(1.4f);
                // FreezeButton.SetVisible(SelectedSlot is StoreSlot);
                int coin = SelectedSlot is StoreSlot ? 3 : SelectedSlot.OwnerHero.GetCost();
                ScreenPrepareContext.Events.ShowHeroInfo?.Invoke(true,SelectedSlot.OwnerHero, 1);
            }

        }

        public void SetCurrentHolderDrag(Holder holder)
        {
            CurrentHolderDrag = holder;
            ScreenPrepareContext.Events.SetSellGroupVisible?.Invoke(SelectedSlot is FieldSlot);
            // FreezeButton.SetVisible(CurrentHolderDrag is StoreSlot);
        }

        public void SetCurrentHolderOverride(Holder holder)
        {
            CurrentHolderOverride = holder;
        }

        public void InActiveSelect()
        {
            SelectedSlot?.SetImageFocus(false);
            SelectedSlot?.OwnerHero?.SetHeroOrder(100);
            SelectedSlot?.OwnerHero?.SetHeroScaleTarget(1);
            SelectedSlot = null;
            // FreezeButton.SetVisible(false);
            
            ScreenPrepareContext.Events.ShowHeroInfo?.Invoke(false, null, 1);
        }
    }
}
