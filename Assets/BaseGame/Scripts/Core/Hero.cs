using System;
using System.Collections.Generic;
using System.Threading;
using Game.Data;
using Combat;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using Game.Manager;
using LitMotion;
using ModestTree;
using R3;
using Spine.Unity;
using TMPro;
using TW.ACacheEverything;
using TW.Reactive.CustomComponent;
using TW.Utility.CustomComponent;
using TW.Utility.Extension;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.Core
{
    [SelectionBase]
    public partial class Hero : ACachedMonoBehaviour
    {
        [Inject] public InGameHandleManager InGameHandleManager { get; set; }
        [Inject] public AbilityResolveManager AbilityResolveManager { get; set; }
        [Inject] public FactoryManager FactoryManager { get; set; }
        [field: SerializeField] public HeroConfigData HeroConfigData { get; private set; }
        [field: SerializeField] public HeroSpriteGraphic HeroSpriteGraphic { get; private set; }
        [field: SerializeField] public ReactiveValue<int> Pieces { get; private set; }
        [field: SerializeField] public ReactiveValue<int> AttackDamageBuff { get; private set; }
        [field: SerializeField] public ReactiveValue<int> HealthPointBuff { get; private set; }
        [field: SerializeField] public ReactiveValue<int> AttackDamage { get; private set; }
        [field: SerializeField] public ReactiveValue<int> HealthPoint { get; private set; }
        [field: SerializeField] public int HealthOngoing {get; private set;}
        [field: SerializeField] public GameObject HeroStatGroup { get; private set; }
        [field: SerializeField] public GameObject HeroVisibleGroup { get; private set; }
        [field: SerializeField] public StarProcess StarProcess { get; private set; }
        [field: SerializeField] public TextMeshPro TextAttackDamage { get; private set; }
        [field: SerializeField] public TextMeshPro TextHealthPoint { get; private set; }
        [field: SerializeField] public bool IsSelfDespawnAfterDrag { get; private set; }
        [field: SerializeField] public HeroAnim HeroAnim { get; private set; }
        private Vector3 TargetPosition { get; set; }
        private float TargetScale { get; set; }
        private float CurrentScale { get; set; } = 1f;
        private CancellationTokenSource CancellationTokenSource { get; set; }
        private MotionHandle ScaleMotionHandle { get; set; }
        public int FaintLoopTime { get; private set; }
        public Holder LastHolder { get; set; }
        public class Factory : PlaceholderFactory<Object, Hero>
        {
            public static Factory CreateInstance()
            {
                return new Factory();
            }
        }

        private void Awake()
        {
            Pieces.ReactiveProperty.Subscribe(OnHeroPiecesChange).AddTo(this);
            AttackDamage.ReactiveProperty.Subscribe(OnHeroAttackDamageChange).AddTo(this);
            HealthPoint.ReactiveProperty.Subscribe(OnHeroHealthPointChange).AddTo(this);
            AttackDamageBuff.ReactiveProperty.Subscribe(OnHeroAttackDamageBuffChange).AddTo(this);
            HealthPointBuff.ReactiveProperty.Subscribe(OnHeroHealthPointBuffChange).AddTo(this);
        }

        private void OnDestroy()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;

            ScaleMotionHandle.TryCancel();
        }

        public Hero InitBaseStat()
        {
            Pieces.Value = 1;
            AttackDamage.Value = HeroConfigData.AttackDamage;
            HealthPoint.Value = HeroConfigData.HealthPoint;
            HealthPointBuff.Value = 0;
            AttackDamageBuff.Value = 0;
            HealthOngoing =  HealthPoint.Value + HealthPointBuff.Value;
            FaintLoopTime = 0;
            HeroConfigData.Initialize(InGameHandleManager, FactoryManager);
            return this;
        }
        public Hero InitStat(InGameHeroData inGameHeroData)
        {
            Pieces.Value = inGameHeroData.Pieces;
            AttackDamageBuff.Value = inGameHeroData.AttackDamageBuff;
            HealthPointBuff.Value = inGameHeroData.HealthPointBuff;
            HealthOngoing =  HealthPoint.Value + HealthPointBuff.Value;
            return this;
        }
        public Hero InitInstanceStat(int star, int attack, int health)
        {
            Pieces.Value = star switch
            {
                1 => 1,
                2 => 3,
                3 => 6,
                _ => 1
            };
            AttackDamageBuff.Value = attack - AttackDamage;
            HealthPointBuff.Value = health - HealthPoint;
            HealthOngoing =  health;
            return this;
        }
        public Hero InitInstanceStat(int star)
        {
            Pieces.Value = star switch
            {
                1 => 1,
                2 => 3,
                3 => 6,
                _ => 1
            };
            return this;
        }
        public Hero SetHolder(Holder holder)
        {
            LastHolder = holder;
            return this;
        }
        public Hero SetFlip(bool isFlip)
        {
            HeroAnim.SetFlip(isFlip);
            return this;
        }
        
        public Hero SetVisible(bool isVisible)
        {
            HeroVisibleGroup.SetActive(isVisible);
            return this;
        }

        public Hero SetHeroOrder(int order)
        {
            HeroAnim.SetHeroOrder(order);
            return this;
        }

        public Hero SetHeroPiece(int value)
        {
            Pieces.Value = value;
            return this;
        }
        
        public Hero SetFaintLoopTime(int value)
        {
            FaintLoopTime = value;
            return this;
        }

        public Hero SetHeroStarProcess(bool isShow)
        {
            StarProcess.gameObject.SetActive(isShow);
            return this;
        }

        public Hero SetHeroStatGroup(bool isShow)
        {
            HeroStatGroup.SetActive(isShow);
            return this;
        }

        private void OnHeroPiecesChange(int piece)
        {
            StarProcess.SetPieces(piece);
        }

        private void OnHeroAttackDamageChange(int value)
        {
            TextAttackDamage.text = value.ToString();
        }

        private void OnHeroHealthPointChange(int value)
        {
            TextHealthPoint.text = value.ToString();
        }

        private void OnHeroAttackDamageBuffChange(int value)
        {
            AttackDamage.Value = HeroConfigData.AttackDamage + value;
        }

        private void OnHeroHealthPointBuffChange(int value)
        {
            HealthPoint.Value = HeroConfigData.HealthPoint + value;
        }

        public Hero SetHeroInstancePosition(Vector3 position)
        {
            TargetPosition = position;
            Transform.position = position;
            return this;
        }

        public Hero SetHeroTargetPosition(Vector2 position)
        {
            TargetPosition = position;
            return this;
        }

        public Hero SetHeroScaleTarget(float scale)
        {
            TargetScale = scale;
            ScaleMotionHandle.TryCancel();
            ScaleMotionHandle = LMotion.Create(CurrentScale, TargetScale, 0.2f)
                .WithEase(Ease.OutCubic)
                .Bind(ScaleMotionUpdateCache);
            return this;


        }
        [ACacheMethod]
        private void ScaleMotionUpdate(float value)
        {
            CurrentScale = value;
            Transform.localScale = new Vector3(value, value, 1);
        }
        public async UniTask StartDragUpdate()
        {
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = new CancellationTokenSource();
            SetHeroOrder(200);
            SetHeroScaleTarget(1.4f);
            await foreach (AsyncUnit _ in UniTaskAsyncEnumerable.EveryUpdate()
                               .WithCancellation(CancellationTokenSource.Token))
            {
                if (IsCompleteMoveToTarget()) continue;
                Vector3 newPosition = Vector3.Lerp(Transform.position, TargetPosition, 0.1f);
                Transform.position = newPosition;
            }
        }

        public async UniTask StopDragUpdate()
        {
            await UniTask.WaitUntil(IsCompleteMoveToTargetCache, cancellationToken: CancellationTokenSource.Token);
            SetHeroOrder(100);
            SetHeroScaleTarget(1f);
            CancellationTokenSource?.Cancel();
            CancellationTokenSource?.Dispose();
            CancellationTokenSource = null;
            if (IsSelfDespawnAfterDrag)
            {
                Destroy(gameObject);
            }
        }
        
        [ACacheMethod]
        private bool IsCompleteMoveToTarget()
        {
            return Vector3.Distance(Transform.position, TargetPosition) < 0.01f;
        }

        public bool IsMaxStar()
        {
            return Pieces.Value >= 6;
        }

        public int GetStar()
        {
            return Pieces.Value switch
            {
                1 => 1,
                2 => 1,
                3 => 2,
                4 => 2,
                5 => 2,
                6 => 3,
                _ => 1
            };
        }

        public Hero Fusion(Hero hero)
        {
            Pieces.Value = Mathf.Clamp(Pieces.Value + hero.Pieces.Value, 1, 6);
            AttackDamageBuff.Value = Mathf.Max(AttackDamageBuff.Value, hero.AttackDamageBuff.Value) + 1;
            HealthPointBuff.Value = Mathf.Max(HealthPointBuff.Value, hero.HealthPointBuff.Value) + 1;


            return this;
        }

        public Hero SetSelfDespawnAfterDrag(bool value)
        {
            IsSelfDespawnAfterDrag = value;
            return this;
        }

        public void SelfDespawnImmediate()
        {
            Destroy(gameObject);
        }

        public int GetCost()
        {
            return Pieces.Value switch
            {
                1 => 1,
                2 => 1,
                3 => 2,
                4 => 2,
                5 => 2,
                6 => 3,
                _ => 1
            };
        }
    }

    public partial class Hero
    {
        public bool IsFainted => HealthPoint.Value == 0 || HealthOngoing == 0;
        public async UniTask MoveToBattleSlot(BattleSlot from, BattleSlot to, TickRate tickRate,
            CancellationToken cancellationToken, Vector3[] points)
        {
            from.RemoveHero(this);
            await MoveFollowPoints(tickRate, cancellationToken, points);
            to.AddHero(this);
        }

        private async UniTask MoveFollowPoints(TickRate tickRate, CancellationToken cancellationToken, Vector3[] points)
        {
            for (int i = 1; i < points.Length; i++)
            {
                await HeroAnim.JumpToPosition(points[i], tickRate, cancellationToken);
            }
        }
        
        public async UniTask Attack(Hero target, TickRate tickRate, CancellationToken cancellationToken)
        {
            target.WillTakeDamage(AttackDamage.Value);
            await HeroAnim.OnAttack(OnHitImpact, tickRate, cancellationToken);
            return;

            async UniTask OnHitImpact()
            {
                await target.TakeDamage(AttackDamage, Transform.position, target.Transform.position, tickRate, cancellationToken);
            }
        }
        public void WillTakeDamage(int damage)
        {
            HealthOngoing = Mathf.Max(0, HealthOngoing - damage);
        }
        public async UniTask TakeDamage(int damage, Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            int currentHealth = Mathf.Max(0, HealthPoint.Value - damage);
            HealthPoint.Value = currentHealth;
            if (damage == 0) return;
            FactoryManager.CreateDamageNumberMesh(Transform.position, damage);
            Debug.Log($"{HeroConfigData.name} TakeDamage {damage} => {currentHealth}");
            if (currentHealth > 0)
            {
                await Hurt(startPosition, endPosition, tickRate, cancellationToken);
            }
            else
            {
                await Faint(tickRate, cancellationToken);
            }
        }


        public async UniTask GainStat(int attack, int health, Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            HealthPointBuff.Value = Mathf.Min(99 -HeroConfigData.HealthPoint, HealthPointBuff.Value + health);
            AttackDamageBuff.Value = Mathf.Min(99 -HeroConfigData.AttackDamage, AttackDamageBuff.Value + attack);
            HealthOngoing = HealthPoint.Value;
            if (health != 0 || attack != 0)
            {
                FactoryManager.CreateGainStatNumberMesh(Transform.position, attack, health);
                await OnImpact(startPosition, endPosition, tickRate, cancellationToken);
            }
        }
        public async UniTask RemoveStat(int attack, int health, Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            HealthPointBuff.Value = Mathf.Max(-HeroConfigData.HealthPoint, HealthPointBuff.Value - health);
            AttackDamageBuff.Value = Mathf.Max(-HeroConfigData.AttackDamage, AttackDamageBuff.Value - attack);
            HealthOngoing = HealthPoint.Value;
            if (health != 0 || attack != 0)
            {
                FactoryManager.CreateRemoveStatNumberMesh(Transform.position, attack, health);
                await OnImpact(startPosition, endPosition, tickRate, cancellationToken);
            }
        }
        public async UniTask RemoveStatPercent(int attackPercent, int healthPercent, Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            int healthRemove = (int)(HealthPoint.Value * (healthPercent / 100f));
            int attackRemove = (int)(AttackDamage.Value * (attackPercent / 100f));
            await RemoveStat(attackRemove, healthRemove, startPosition, endPosition, tickRate, cancellationToken);
        }
        
        public async UniTask GainExp(int exp, Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            int oldStar = GetStar();
            Pieces.Value = Mathf.Min(6, Pieces.Value + exp);
            int newStar = GetStar();
            if (exp != 0)
            {
                FactoryManager.CreateGainExpNumberMesh(Transform.position, exp);
                await OnImpact(startPosition, endPosition, tickRate, cancellationToken);
                if (oldStar < newStar)
                {
                    TryTriggerAbility<SelfStarUpTrigger>(tickRate, cancellationToken);
                }
            }
        }

        private async UniTask Hurt(Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            TryTriggerAbility<SelfHurtTrigger>(tickRate, cancellationToken);
            await HeroAnim.OnImpact(startPosition, endPosition, tickRate, cancellationToken);
        }
        private async UniTask Faint(TickRate tickRate, CancellationToken cancellationToken)
        {
            for (int i = 0; i < FaintLoopTime + 1; i++)
            {
                TryTriggerAbility<SelfFaintTrigger>(tickRate, cancellationToken);
            }
            
            InGameHandleManager.TryTriggerAllyAbility<AllyFaintTrigger>(this, tickRate, cancellationToken);
            if (HasTriggerEffect<SelfFaintTrigger>())
            {
                InGameHandleManager.TryTriggerAllyAbility<AllyTriggerFaintAbilityTrigger>(this, tickRate, cancellationToken);
            }
            await HeroAnim.OnFaint(tickRate, cancellationToken);
            InGameHandleManager.RemoveHero(this);
            SetVisible(false);
        }

        private async UniTask OnImpact(Vector3 startPosition, Vector3 endPosition, TickRate tickRate,
            CancellationToken cancellationToken)
        {
            await HeroAnim.OnImpact(startPosition, endPosition, tickRate, cancellationToken);
        }
        
        public void TryTriggerAbility<T>(TickRate tickRate, CancellationToken cancellationToken) where T : AbilityTrigger
        {
            if (!HasTriggerEffect<T>()) return;
            AbilityResolveManager.Resolve(this, HeroConfigData.GetAbilityPower(GetStar()), tickRate, cancellationToken);
        }
        public bool HasTriggerEffect<T>() where T : AbilityTrigger
        {
            return HeroConfigData.GetAbilityPower(GetStar()).HasTrigger<T>();
        }
    }


    public partial class Hero
    {
        public void SetHeroConfigData(HeroConfigData heroConfigData)
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
            HeroConfigData = heroConfigData;
            HeroSpriteGraphic.Init(HeroConfigData);

            TextAttackDamage.text = heroConfigData.AttackDamage.ToString();
            TextHealthPoint.text = heroConfigData.HealthPoint.ToString();
            HeroAnim.SkeletonAnimController.EditorSetup();
        }
    }
}