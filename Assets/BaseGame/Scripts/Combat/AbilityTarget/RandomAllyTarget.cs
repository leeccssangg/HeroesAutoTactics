using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/RandomAllyTarget")]
    public class RandomAllyTarget : AbilityTarget
    {
        [field: SerializeField] public int MaxTarget { get; private set; }

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
            Holder[] allyFieldSlot = FieldSlots.Shuffle(InGameHandleManager.Random);
            holders = new Holder[20];
            count = 0;
            foreach (Holder fieldSlot in allyFieldSlot)
            {
                if (!fieldSlot.HasHero) continue;
                if (fieldSlot.OwnerHero.IsFainted) continue;
                if (fieldSlot.OwnerHero == hero) continue;
                holders[count] = fieldSlot;
                count++;
                if (count >= MaxTarget) break;
            }

            return count > 0;
        }

        private bool TryFindTargetInBattleState(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] allyBattleSlot = GetAllyBattleSlot(hero).Shuffle(InGameHandleManager.Random);
            holders = new Holder[20];
            count = 0;
            foreach (Holder battleSlot in allyBattleSlot)
            {
                if (!battleSlot.HasHero) continue;
                if (battleSlot.OwnerHero.IsFainted) continue;
                if (battleSlot.OwnerHero == hero) continue;
                holders[count] = battleSlot;
                count++;
                if (count >= MaxTarget) break;
            }

            return count > 0;
        }
    }
}