using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;

namespace Combat
{
    public class NonePower : AbilityPower
    {
        public override async UniTask Resolve(Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            await UniTask.CompletedTask;
        }
    }
}