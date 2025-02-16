using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Combat
{
    [CreateAssetMenu(menuName = "AbilityPower/DealDamagePower")]
    public class DealDamagePower : AbilityPower
    {
        [field: SerializeField] public int Damage { get; private set; }
        [field: SerializeField] public int LoopTime { get; private set; }
        [field: SerializeField] public Projectile Projectile { get; private set; }
        private HeroToHeroCallback m_OnProjectileMoveCompleteFunc;
        private HeroToHeroCallback OnProjectileMoveCompleteFunc => m_OnProjectileMoveCompleteFunc ??= OnProjectileMoveComplete;
        public override AbilityPower UpdateData(Dictionary<string, string> data)
        {
            base.UpdateData(data);
            if (int.TryParse(data["Value0"], out int value0))
            {
                Damage = value0;
            }

            if (int.TryParse(data["Value1"], out int value1))
            {
                LoopTime = value1;
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
            List<UniTask> taskList = new();
            for (int i = 0; i < LoopTime; i++)
            {
                if (!AbilityTarget.TryFindTarget(hero, out Holder[] holders, out int count)) return;

                for (int j = 0; j < count; j++)
                {
                    Projectile projectile = FactoryManager.CreateProjectile(Projectile);
                    projectile.Transform.position = hero.Transform.position;
                    UniTask task = projectile.Move(hero, holders[j].OwnerHero, OnProjectileMoveCompleteFunc, tickRate, cancellationToken);
                    taskList.Add(task);
                    
                    holders[j].OwnerHero.WillTakeDamage(Damage);
                    await UniTask.DelayFrame((int)(1+ 4 / tickRate.ToValue()), cancellationToken: cancellationToken);
                }
            }
            await UniTask.WhenAll(taskList);
        }

        private async UniTask OnProjectileMoveComplete(Hero from, Hero to, TickRate tickRate, CancellationToken cancellationToken)
        {
            await to.TakeDamage(Damage, from.Transform.position, to.Transform.position, tickRate, cancellationToken);
        }
    }
}