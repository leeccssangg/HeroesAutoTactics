using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityPower/GiveFreeRollPower")]
    public class GiveFreeRollPower : AbilityPower
    {
        [field: SerializeField] public int FreeRollGain {get; private set;}
        public override AbilityPower UpdateData(Dictionary<string, string> data)
        {
            base.UpdateData(data);
            if (int.TryParse(data["Value0"], out int freeRollGain))
            {
                FreeRollGain = freeRollGain;
            }

            return this;
        }

        public override UniTask Resolve(Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            InGameHandleManager.AddFreeRollCount(FreeRollGain);
            return UniTask.CompletedTask;
        }
    }
}