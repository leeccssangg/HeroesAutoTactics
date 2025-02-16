using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/HeroBehindTarget")]
    public class HeroBehindTarget : AbilityTarget
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
            holders = new Holder[20];
            count = 0;
            bool isHeroFound = false;
            for (int i = 0; i < FieldSlots.Length; i++)
            {
                if (!FieldSlots[i].HasHero) continue;
                if (FieldSlots[i].OwnerHero.IsFainted) continue;
                if (FieldSlots[i] == hero.LastHolder)
                {
                    isHeroFound = true;
                }
                else if (isHeroFound)
                {
                    holders[count] = FieldSlots[i];
                    count++;
                }

                if (count >= MaxTarget) break;
            }

            return count > 0;
        }

        private bool TryFindTargetInBattleState(Hero hero, out Holder[] holders, out int count)
        {
            Holder[] allyBattleSlot = GetAllyBattleSlot(hero);
            holders = new Holder[20];
            count = 0;
            bool isHeroFound = false;
            for (int i = 0; i < allyBattleSlot.Length; i++)
            {
                if (!allyBattleSlot[i].HasHero) continue;
                if (allyBattleSlot[i].OwnerHero.IsFainted) continue;
                if (allyBattleSlot[i] == hero.LastHolder)
                {
                    isHeroFound = true;
                }
                else if (isHeroFound)
                {
                    holders[count] = allyBattleSlot[i];
                    count++;
                }

                if (count >= MaxTarget) break;
            }

            return count > 0;
        }
    }
}