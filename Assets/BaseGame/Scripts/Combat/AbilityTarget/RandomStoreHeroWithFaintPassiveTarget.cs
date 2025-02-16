using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/RandomStoreHeroWithFaintPassiveTarget")]
    public class RandomStoreHeroWithFaintPassiveTarget : AbilityTarget
    {
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {
            holders = new Holder[20];
            count = 0;
            if (InGameHandleManager.IsInBattleState()) return false;
            if (!InGameHandleManager.IsInPrepareState()) return false;
            
            foreach (Holder storeSlot in StoreSlots)
            {
                if (!storeSlot.HasHero) continue;
                if (!storeSlot.OwnerHero.HasTriggerEffect<AbilityTrigger>()) continue;
                holders[count] = storeSlot;
                count++;
            }
            return count > 0;
        }
    }
}