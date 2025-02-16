using System;
using Game.Core;
using Game.Manager;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityTarget/NoneTarget")]
    public class NoneTarget : AbilityTarget
    {
        public override bool TryFindTarget(Hero hero, out Holder[] holders, out int count)
        {
            holders = Array.Empty<Holder>();
            count = 0;
            return false;
        }
    }

}