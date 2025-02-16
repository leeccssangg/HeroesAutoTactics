using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/AllAllyIncludeSelfTarget")]
    public class AllAllyIncludeSelfTarget : AbilityTarget
    {
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {

            if (InGameHandleManager.IsInPrepareState())
            {
                return TryFindTargetInPrepareState(hero, out holders, out count);
            }

            if (InGameHandleManager.IsInBattleState())
            {
                return TryFindTargetInBattleState(hero, out holders, out count);
            }
            
            holders = new Holder[20];
            count = 0;
            return false;
        }

        private bool TryFindTargetInPrepareState(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] allyFieldSlot = FieldSlots;
            holders = new Holder[20];
            count = 0;
            foreach (Holder fieldSlot in allyFieldSlot)
            {
                if (!fieldSlot.HasHero) continue;
                if (fieldSlot.OwnerHero.IsFainted) continue;
                holders[count] = fieldSlot;
                count++;
            }

            return count > 0;
        }

        private bool TryFindTargetInBattleState(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] allyBattleSlot = GetAllyBattleSlot(hero);
            holders = new Holder[20];
            count = 0;
            foreach (Holder battleSlot in allyBattleSlot)
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