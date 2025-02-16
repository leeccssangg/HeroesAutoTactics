using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/FirstAllyTarget")]
    public class AllyAtPositionTarget : AbilityTarget
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
            if (InGameHandleManager.IsInPrepareState())
            {
                return Position switch
                {
                    PositionFind.First => TryFindTargetAtFirstInPrepareState(hero, out holders, out count),
                    PositionFind.Last => TryFindTargetAtLastInPrepareState(hero, out holders, out count),
                    _ => count > 0
                };
            }
            if (InGameHandleManager.IsInBattleState())
            {
                return Position switch
                {
                    PositionFind.First => TryFindTargetAtFirstInBattleState(hero, out holders, out count),
                    PositionFind.Last => TryFindTargetAtLastInBattleState(hero, out holders, out count),
                    _ => count > 0
                };
            }
            
            return false;
        }
        private bool TryFindTargetAtFirstInPrepareState(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] fieldSlots = FieldSlots;
            holders = new Holder[20];
            count = 0;
            foreach (Holder battleSlot in fieldSlots)
            {
                if (!battleSlot.HasHero) continue;
                if (battleSlot.OwnerHero.IsFainted) continue;
                holders[count] = battleSlot;
                count++;
                if (count >= MaxTarget) break;
            }
            return count > 0;
        }
        private bool TryFindTargetAtLastInPrepareState(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] fieldSlots = FieldSlots;
            holders = new Holder[20];
            count = 0;
            for (int i = fieldSlots.Length - 1; i >= 0; i--)
            {
                Holder battleSlot = fieldSlots[i];
                if (!battleSlot.HasHero) continue;
                if (battleSlot.OwnerHero.IsFainted) continue;
                holders[count] = battleSlot;
                count++;
                if (count >= MaxTarget) break;
            }
            return count > 0;
        }
        private bool TryFindTargetAtFirstInBattleState(Hero hero, out Holder[] holders, out int count)
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
                if (count >= MaxTarget) break;
            }
            return count > 0;
        }
        private bool TryFindTargetAtLastInBattleState(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] allyBattleSlot = GetAllyBattleSlot(hero);
            holders = new Holder[20];
            count = 0;
            for (int i = allyBattleSlot.Length - 1; i >= 0; i--)
            {
                Holder battleSlot = allyBattleSlot[i];
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