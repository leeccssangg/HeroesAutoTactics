using System;
using DamageNumbersPro;
using Game.Core;
using Game.GlobalConfig;
using R3;
using R3.Triggers;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;

namespace Game.Manager
{
    public class FactoryManager : Singleton<FactoryManager>
    {
        [Inject] private Hero.Factory HeroFactory { get; set; }
        [Inject] private Projectile.Factory ProjectileFactory { get; set; }
        [Inject] private CombatText.Factory CombatTextFactory { get; set; }

        [field: SerializeField] public CombatText DamageCombatText { get; private set; }
        [field: SerializeField] public CombatText GainStatAttackText { get; private set; }
        [field: SerializeField] public CombatText GainStatHealthText { get; private set; }
        // [field: SerializeField] public CombatText GainStatAttackAndHealthText { get; private set; }
        [field: SerializeField] public DamageNumber DamageNumberMesh {get; private set;}
        [field: SerializeField] public DamageNumber GainAttackNumberMesh {get; private set;}
        [field: SerializeField] public DamageNumber GainHealthNumberMesh {get; private set;}
        [field: SerializeField] public DamageNumber RemoveAttackNumberMesh {get; private set;}
        [field: SerializeField] public DamageNumber RemoveHealthNumberMesh {get; private set;}
        [field: SerializeField] public DamageNumber GainExpNumberMesh {get; private set;}
        private void Start()
        {
            // this.UpdateAsObservable().Where(_ =>Input.GetKeyDown(KeyCode.Mouse0))
            //     .Subscribe(_ =>
            //     {
            //         Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //         position.z = 0;
            //         CreateDamageNumberMesh(position, UnityEngine.Random.Range(1, 100));
            //     });
        }

        public Hero CreateHero(int heroId)
        {
            HeroConfigData heroConfigData = HeroPoolGlobalConfig.Instance.GetHeroConfigData(heroId);
            Hero hero = HeroFactory.Create(heroConfigData.HeroPrefab);
            hero.Transform.SetParent(Transform);
            return hero;
        }

        public Hero CreateRandomHero(int round)
        {
            HeroConfigData heroConfigData = HeroPoolGlobalConfig.Instance.GetRandomHeroConfigData(round);
            Hero hero = HeroFactory.Create(heroConfigData.HeroPrefab);
            hero.Transform.SetParent(Transform);
            return hero;
        }

        public Projectile CreateProjectile(Projectile projectilePrefab)
        {
            return ProjectileFactory.Create(projectilePrefab);
        }
        
        public void CreateDamageNumberMesh(Vector3 position, int damage)
        {
            DamageNumberMesh.Spawn(position + Vector3.up * 3f, damage);
        }
        public void CreateGainStatNumberMesh(Vector3 position, int attack, int health)
        {
            if (attack > 0 && health > 0)
            {
                GainAttackNumberMesh.Spawn(position + Vector3.up * 3f + Vector3.left, attack);
                GainHealthNumberMesh.Spawn(position + Vector3.up * 3f + Vector3.right, health);
            }

            else if (attack > 0)
            {
                GainAttackNumberMesh.Spawn(position + Vector3.up * 3f, attack);
            }
            
            else if (health > 0)
            {
                GainHealthNumberMesh.Spawn(position + Vector3.up * 3f, health);
            }
        }
        public void CreateRemoveStatNumberMesh(Vector3 position, int attack, int health)
        {
            if (attack > 0 && health > 0)
            {
                RemoveAttackNumberMesh.Spawn(position + Vector3.up * 3f + Vector3.left, attack);
                RemoveHealthNumberMesh.Spawn(position + Vector3.up * 3f + Vector3.right, health);
            }

            else if (attack > 0)
            {
                RemoveAttackNumberMesh.Spawn(position + Vector3.up * 3f, attack);
            }
            
            else if (health > 0)
            {
                RemoveHealthNumberMesh.Spawn(position + Vector3.up * 3f, health);
            }
        }
        public void CreateGainExpNumberMesh(Vector3 position, int heal)
        {
            GainExpNumberMesh.Spawn(position + Vector3.up * 3f, heal);
        }
    }
}
