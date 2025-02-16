using Game.Core;
using Game.Manager;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/OpponentAtPositionTarget")]
    public class OpponentAtPositionTarget : AbilityTarget
    {
        public enum PositionFind
        {
            First,
            Last,
        }
        [field: SerializeField] public PositionFind Position {get; private set;}
        [field: SerializeField] public int MaxTarget {get; private set;}
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {
            holders = new Holder[20];
            count = 0;
            if (InGameHandleManager.IsInPrepareState()) return false;
            if (!InGameHandleManager.IsInBattleState()) return false;

            return Position switch
            {
                PositionFind.First => TryFindTargetAtFirst(hero, out holders, out count),
                PositionFind.Last => TryFindTargetAtLast(hero, out holders, out count),
                _ => count > 0
            };
        }
        private bool TryFindTargetAtFirst(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] opponentBattleSlot = GetOpponentBattleSlot(hero);
            holders = new Holder[20];
            count = 0;
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
        private bool TryFindTargetAtLast(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] opponentBattleSlot = GetOpponentBattleSlot(hero);
            holders = new Holder[20];
            count = 0;
            for (int i = opponentBattleSlot.Length - 1; i >= 0; i--)
            {
                Holder battleSlot = opponentBattleSlot[i];
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