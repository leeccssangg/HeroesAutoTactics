using System.Collections.Generic;
using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/SummonedTarget")]
    public class SummonedTarget : AbilityTarget
    {
        public Queue<Hero> TargetQueue { get; } = new Queue<Hero>();
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {
            holders = new Holder[1];
            Hero summonedHero = TargetQueue.Dequeue();
            holders[0] = summonedHero.LastHolder;
            count = summonedHero.IsFainted ? 0 : 1;
            return count == 1;
        }
    }
}