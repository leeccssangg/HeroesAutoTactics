using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

namespace Combat
{
    public class GiveCoinPower : AbilityPower
    {
        [field: SerializeField] public int CoinGain { get; private set; }
        public override AbilityPower UpdateData(Dictionary<string, string> data)
        {
            base.UpdateData(data);
            if (int.TryParse(data["Value0"], out int coinGain))
            {
                CoinGain = coinGain;
            }

            return this;
        }

        public override UniTask Resolve(Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            InGameHandleManager.AddCoin(CoinGain);
            return UniTask.CompletedTask;
        }
    }
}