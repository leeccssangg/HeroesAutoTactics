using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/SelfOnlyTarget")]
    public class SelfOnlyTarget : AbilityTarget
    {
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {
            holders = new Holder[1];
            count = 1;
            holders[0] = hero.LastHolder;
            if (hero.IsFainted) return false;
            return true;
        }
    }
}