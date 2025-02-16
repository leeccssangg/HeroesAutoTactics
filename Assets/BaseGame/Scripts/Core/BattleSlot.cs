using Game.Data;
using Game.Manager;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

namespace Game.Core
{
    public class BattleSlot : Holder
    {
        [Inject] private FactoryManager FactoryManager { get; set; }
        [field: SerializeField] public int SlotIndex { get; private set; }
        [field: SerializeField] public bool IsFlip { get; private set; }
        [field: SerializeField] public InGameHeroData InGameHeroData { get; private set; }

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
        }

        public override void OnPointerExit(PointerEventData eventData)
        {
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
        }

        public override void OnDrag(PointerEventData eventData)
        {
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
        }

        #endregion
        public bool TryGetHero(out Hero hero)
        {
            hero = OwnerHero;
            return HasHero;
        }
        public void InitBattleSlot(InGameHeroData inGameHeroData)
        {
            InGameHeroData = inGameHeroData;
            if (inGameHeroData == null) return;
            if (inGameHeroData.HeroId == -1) return;
            InitNewHeroImmediate(FactoryManager.CreateHero(InGameHeroData.HeroId)
                .InitBaseStat()
                .InitStat(InGameHeroData)
                .SetFlip(IsFlip));
        }

        public void ClearBattleSlot()
        {
            if (OwnerHero == null) return;
            Destroy(OwnerHero.gameObject);
        }
    }
}