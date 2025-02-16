using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Manager;
using UnityEditor;
using UnityEngine;

namespace Combat
{
    public class SummonAndGiveExpPower : AbilityPower
    {
        [field: SerializeField] public Projectile Projectile {get; private set;}
        [field: SerializeField] public int Amount {get; private set;}
        [field: SerializeField] public int UnitId {get; private set;}
        [field: SerializeField] public int Level {get; private set;}
        [field: SerializeField] public int Attack {get; private set;}
        [field: SerializeField] public int Health {get; private set;}
        [field: SerializeField] public int Exp {get; private set;}
        
        private HeroToHolderCallback m_OnProjectileMoveCompleteFunc;
        private HeroToHolderCallback OnProjectileMoveCompleteFunc => m_OnProjectileMoveCompleteFunc ??= OnProjectileMoveComplete;
        
        private HeroToHolderCallback m_OnProjectileGiveExpMoveCompleteFunc;
        private HeroToHolderCallback OnProjectileGiveExpMoveCompleteFunc => m_OnProjectileGiveExpMoveCompleteFunc ??= OnProjectileGiveExpMoveComplete;
        public override AbilityPower UpdateData(Dictionary<string, string> data)
        {
            base.UpdateData(data);
            if (int.TryParse(data["Value0"], out int amount))
            {
                Amount = amount;
            }
            if (int.TryParse(data["Value1"], out int unitId))
            {
                UnitId = unitId;
            }
            if (int.TryParse(data["Value2"], out int level))
            {
                Level = level;
            }
            if (int.TryParse(data["Value3"], out int attack))
            {
                Attack = attack;
            }
            if (int.TryParse(data["Value4"], out int health))
            {
                Health = health;
            }
            if (int.TryParse(data["Value5"], out int exp))
            {
                Exp = exp;
            }
            if (Projectile == null)
            {
                Projectile = AssetDatabase.LoadAssetAtPath<Projectile>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:Prefab ProjectileBase")[0]));
            }
            return this;
        }
        
        public override async UniTask Resolve(Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            Holder[] targetList = await AbilityTarget.GetEmptyHolderToSummon(hero, Amount, tickRate, cancellationToken);
            List<UniTask> taskList = new();
            for (int i = 0; i < targetList.Length; i++)
            {
                Projectile projectile = FactoryManager.CreateProjectile(Projectile);
                projectile.Transform.position = hero.Transform.position;
                UniTask task = projectile.Move(hero, targetList[i], OnProjectileMoveCompleteFunc, tickRate, cancellationToken);
                taskList.Add(task);
                await UniTask.Delay((int)(50 / tickRate.ToValue()), cancellationToken: cancellationToken);
            }
            await UniTask.WhenAll(taskList);
            taskList.Clear();
            for (int i = 0; i < targetList.Length; i++)
            {
                Projectile projectile = FactoryManager.CreateProjectile(Projectile);
                projectile.Transform.position = hero.Transform.position;
                UniTask task = projectile.Move(hero, targetList[i], OnProjectileGiveExpMoveCompleteFunc, tickRate, cancellationToken);
                taskList.Add(task);
                await UniTask.Delay((int)(50 / tickRate.ToValue()), cancellationToken: cancellationToken);
            }
            await UniTask.WhenAll(taskList);
        }

        private UniTask OnProjectileMoveComplete(Hero fromHero, Holder toHolder, TickRate tickRate, CancellationToken cancellationToken)
        {
            Hero summonHero = FactoryManager.CreateHero(UnitId);
            summonHero.Transform.position = toHolder.Transform.position;
            summonHero.InitBaseStat().InitInstanceStat(Level, Attack, Health); 
            toHolder.ForceAddHeroInstance(summonHero);
            return UniTask.CompletedTask;
        }
        private async UniTask OnProjectileGiveExpMoveComplete(Hero fromHero, Holder toHolder, TickRate tickRate, CancellationToken cancellationToken)
        {
            await toHolder.OwnerHero.GainExp(Exp, fromHero.Transform.position, toHolder.Transform.position, tickRate, cancellationToken);
        }
    }
}