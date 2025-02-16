using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/AllOpponentTarget")]
    public class AllOpponentTarget : AbilityTarget
    {
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {
            holders = new Holder[20];
            count = 0;
            if (InGameHandleManager.IsInPrepareState()) return false;
            if (!InGameHandleManager.IsInBattleState()) return false;

            Holder[] opponentBattleSlot = GetOpponentBattleSlot(hero);
            foreach (Holder battleSlot in opponentBattleSlot)
            {
                if (!battleSlot.HasHero) continue;
                if (battleSlot.OwnerHero.IsFainted) continue;
                holders[count] = battleSlot;
                count++;
            }
            return count > 0;
        }
    }
}