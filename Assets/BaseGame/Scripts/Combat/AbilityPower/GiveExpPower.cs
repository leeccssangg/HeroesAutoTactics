using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Combat
{
    public class GiveExpPower : AbilityPower
    {
        [field: SerializeField] public int ExpGain { get; private set; }
        [field: SerializeField] public Projectile Projectile { get; private set; }
        private HeroToHeroCallback m_OnProjectileMoveCompleteFunc;
        private HeroToHeroCallback OnProjectileMoveCompleteFunc => m_OnProjectileMoveCompleteFunc ??= OnProjectileMoveComplete;
        public override AbilityPower UpdateData(Dictionary<string, string> data)
        {
            base.UpdateData(data);
            if (int.TryParse(data["Value0"], out int expGain))
            {
                ExpGain = expGain;
            }
            if (Projectile == null)
            {
                Projectile = AssetDatabase.LoadAssetAtPath<Projectile>(
                    AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Prefab ProjectileBase")[0]));
            }
            return this;
        }

        public override async UniTask Resolve(Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            if (!AbilityTarget.TryFindTarget(hero, out Holder[] holders, out int count)) return;
            List<UniTask> taskList = new();
            for (int j = 0; j < count; j++)
            {
                Projectile projectile = FactoryManager.CreateProjectile(Projectile);
                projectile.Transform.position = hero.Transform.position;
                UniTask task = projectile.Move(hero, holders[j].OwnerHero, OnProjectileMoveCompleteFunc, tickRate, cancellationToken);
                taskList.Add(task);
                await UniTask.Delay((int)(50 / tickRate.ToValue()), cancellationToken: cancellationToken);
            }

            await UniTask.WhenAll(taskList);
        }

        private async UniTask OnProjectileMoveComplete(Hero from, Hero to, TickRate tickRate, CancellationToken cancellationToken)
        {
            await to.GainExp(ExpGain, from.Transform.position, to.Transform.position, tickRate, cancellationToken);
        }
    }
}