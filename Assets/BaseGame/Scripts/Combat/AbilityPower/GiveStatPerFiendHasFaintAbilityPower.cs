using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using UnityEditor;
using UnityEngine;

namespace Combat
{
    public class GiveStatPerFiendHasFaintAbilityPower : AbilityPower
    {
        [field: SerializeField] public int Attack {get; private set;}
        [field: SerializeField] public int Health {get; private set;}
        [field: SerializeField] public Projectile Projectile {get; private set;}
        private HeroToHeroCallback m_OnProjectileMoveCompleteFunc;
        private HeroToHeroCallback OnProjectileMoveCompleteFunc => m_OnProjectileMoveCompleteFunc ??= OnProjectileMoveComplete;
        public override AbilityPower UpdateData(Dictionary<string, string> data)
        {
            base.UpdateData(data);
            if (int.TryParse(data["Value0"], out int attack))
            {
                Attack = attack;
            }
            if (int.TryParse(data["Value1"], out int health))
            {
                Health = health;
            }
            if (Projectile == null)
            {
                Projectile = AssetDatabase.LoadAssetAtPath<Projectile>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Prefab ProjectileBase")[0]));
            }
            return this;
        }

        public override async UniTask Resolve(Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            if (!AbilityTarget.TryFindTarget(hero, out Holder[] holders, out int count)) return;

            
            List<UniTask> taskList = new();
            for (int i = 0; i < count; i++)
            {
                Projectile projectile = FactoryManager.CreateProjectile(Projectile);
                projectile.Transform.position = hero.Transform.position;
                UniTask task = projectile.Move(hero, holders[i].OwnerHero, OnProjectileMoveCompleteFunc, tickRate, cancellationToken);
                taskList.Add(task);
                await UniTask.Delay((int)(50 / tickRate.ToValue()), cancellationToken: cancellationToken);
            }
            await UniTask.WhenAll(taskList);
        }
        async UniTask OnProjectileMoveComplete(Hero from, Hero to, TickRate tickRate, CancellationToken cancellationToken)
        {
            int countHero = 0;
            foreach (Holder holder in AbilityTarget.GetAllyBattleSlot(from))
            {
                if (holder.HasHero && holder.OwnerHero.HasTriggerEffect<AbilityTrigger>())
                {
                    countHero++;
                }
            }
            if (countHero == 0) return;
            await to.GainStat(Attack * countHero, Health * countHero, from.Transform.position, to.Transform.position, tickRate, cancellationToken);
        }
    }
}