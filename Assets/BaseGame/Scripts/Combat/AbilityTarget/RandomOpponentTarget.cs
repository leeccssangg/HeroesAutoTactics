using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/RandomOpponentTarget")]
    public class RandomOpponentTarget : AbilityTarget
    {
        [field: SerializeField] public int MaxTarget {get; private set;}
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {
            holders = new Holder[20];
            count = 0;
            if (InGameHandleManager.IsInPrepareState()) return false;
            if (!InGameHandleManager.IsInBattleState()) return false;
            
            Holder[] opponentBattleSlot = GetOpponentBattleSlot(hero).Shuffle(InGameHandleManager.Random);
            foreach (Holder battleSlot in opponentBattleSlot)
            {
                if (!battleSlot.HasHero) continue;
                if (battleSlot.OwnerHero.IsFainted) continue;
                holders[count] = battleSlot;
                count++;
                if (count >= MaxTarget) break;
            }
            return count > 0;
        }
    }
}