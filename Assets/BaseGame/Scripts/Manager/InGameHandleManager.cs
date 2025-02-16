using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Data;
using Game.DataAccess;
using R3;
using R3.Triggers;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using TW.UGUI.Core.Activities;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Game.Manager
{
    public partial class InGameHandleManager : Singleton<InGameHandleManager>
    {
        [Inject] private DatabaseManager DatabaseManager { get; set; }
        [Inject] private InGameDataManager InGameDataManager { get; set; }
        [Inject] private InputManager InputManager { get; set; }
        [Inject] private FactoryManager FactoryManager { get; set; }
        [Inject] private AbilityResolveManager AbilityResolveManager { get; set; }
        
        [field: SerializeField, StateMachineDebug] public UniTaskStateMachine<InGameHandleManager> StateMachine {get; private set;}
        [field: SerializeField] public ReactiveValue<int> Heart {get; private set;}
        [field: SerializeField] public ReactiveValue<int> Round {get; private set;}
        [field: SerializeField] public ReactiveValue<int> Win {get; private set;}
        [field: SerializeField] public ReactiveValue<int> Coin {get; private set;}
        
        [field:Title(nameof(PrepareGroup))]
        [field: SerializeField] public ReactiveValue<int> FreeRollCount {get; private set;}
        [field: SerializeField] public GameObject PrepareGroup { get; private set; }
        [field: SerializeField] public FieldSlot[] FieldSlotArray { get; private set; }
        [field: SerializeField] public StoreSlot[] StoreSlotArray { get; private set; }
        private List<FieldSlot> Solution { get; set; }
        private List<FieldSlot> Pool { get; set; }
        private List<FieldSlot> Mark { get; set; }
        
        [field:Title(nameof(BattleGroup))]
        [field: SerializeField] public GameObject BattleGroup { get; private set; }
        [field: SerializeField] public BattleTeam PlayerTeam { get; private set; }
        [field: SerializeField] public BattleTeam OpponentTeam { get; private set; }
        [field: SerializeField] public int RandomSeed { get; private set; }
        [field: SerializeField] public TickRate TickRate { get; private set; }
        

        
        [field:Title(nameof(AbilityTarget))]
        [field: SerializeField] public AllAllyTarget AllAllyTarget { get; private set; }
        public BattleTeam[] BattleTeamArray { get; private set; }
        private int CurrentTeam { get; set; }
        public Random Random { get; private set; }
        private BattleData PlayerBattleData { get; set; }
        private BattleData OpponentBattleData { get; set; }
        [field: Title("Test Data")] 
        [field: SerializeField]  private bool IsTest { get; set; }
        [field: SerializeField] private string JsonData {get; set;}
        [field: SerializeField] public List<string> JsonDataList {get; private set;}
        [field: SerializeField] public BattleTestData BattleTestData {get; private set;}
        private void Start()
        {
            InitStateMachine();
            this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha1))
                .Subscribe(_ =>
                {
                    TickRate = TickRate.Normal;
                });
            this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha2))
                .Subscribe(_ =>
                {
                    TickRate = TickRate.Hyper;
                });
            this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha3))
                .Subscribe(_ =>
                {
                    TickRate = TickRate.SuperHyper;
                });
            this.UpdateAsObservable().Where(_ => Input.GetKeyDown(KeyCode.Alpha4))
                .Subscribe(_ =>
                {
                    AddCoin(100);
                });
        }

        [Button]
        public void TestTeamData()
        {
            IsTest = true;
            TryEndPrepareTurn().Forget();
        }
        public void CreateNewGame()
        {
            InitNewGame();
            InitFieldSlot();
            InitAbilityTarget();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            StateMachine.Stop();
        }

        private void InitStateMachine()
        {
            StateMachine = new UniTaskStateMachine<InGameHandleManager>(this);
            StateMachine.RegisterState(InGameSleepState.Instance);
            StateMachine.Run();
        }
        private void InitFieldSlot()
        {
            foreach (FieldSlot fieldSlot in FieldSlotArray)
            {
                fieldSlot.Construct(InputManager, this, FactoryManager, AbilityResolveManager);
            }
        }
        private void InitNewGame()
        {
            StateMachine.RequestTransition(InGameInitState.Instance);
        }
        private void InitAbilityTarget()
        {
            AllAllyTarget.Initialize(this);
        }
        public void InitStoreSlot(StoreSlot[] storeSlots)
        {
            StoreSlotArray = storeSlots;
            foreach (StoreSlot storeSlot in StoreSlotArray)
            {
                storeSlot.Construct(InputManager, this, FactoryManager, AbilityResolveManager);
            }
        }
        public bool IsInPrepareState()
        {
            return StateMachine.IsCurrentState(InGamePrepareState.Instance);
        }
        public bool IsInBattleState()
        {
            return StateMachine.IsCurrentState(InGameBattleState.Instance);
        }
        public bool IsEnoughCoin(int value)
        {
            return Coin.Value >= value;
        }
        public void ConsumedCoin(int value)
        {
            Coin.Value -= value;
        }
        public void AddCoin(int value)
        {
            Coin.Value += value;
        }
        public void AddFreeRollCount(int value)
        {
            FreeRollCount.Value += value;
        }
        public void ConsumedFreeRollCount(int value)
        {
            FreeRollCount.Value -= value;
        }
        private void LoadPlayerFieldSlotData()
        {
            for (int i = 0; i < FieldSlotArray.Length; i++)
            {
                FieldSlotArray[i].LoadFieldSlot(PlayerBattleData.InGameTeamData.InGameHeroDataArray[i]);
            }
        }
        public async UniTask TryEndPrepareTurn()
        {
            if (IsTest)
            {
                BattleTestData = new BattleTestData(JsonData);
                PlayerBattleData = BattleTestData.PlayerData;
                OpponentBattleData = BattleTestData.OpponentData;
                Heart.Value = BattleTestData.Heart;
                Round.Value = BattleTestData.Round;
                Win.Value = BattleTestData.Win;
            }
            else
            {
                PlayerBattleData = new BattleData(InGameDataManager.UserData.Rank, Win, Heart, Round, GetInGameTeamData());
                PostPlayerBattleDataAccess.Response response1 = await DatabaseManager.PostPlayerBattleData(PlayerBattleData);
                if (response1.IsFaulted)
                {
                    ActivityOptions activityOptions = new ActivityOptions(nameof(ActivityConnectionError));
                    Action onTryAgain = OnTryAgain;
                    await ActivityContainer.Find(ContainerKey.OverlayActivity).ShowAsync(activityOptions, onTryAgain);
                    return;
                }
                GetOpponentBattleDataAccess.Response response2 = await DatabaseManager.GetOpponentBattleData(InGameDataManager.UserData.Rank, Win, Heart, Round);
                if (response2.IsFaulted)
                {
                    ActivityOptions activityOptions = new ActivityOptions(nameof(ActivityConnectionError));
                    Action onTryAgain = OnTryAgain;
                    await ActivityContainer.Find(ContainerKey.OverlayActivity).ShowAsync(activityOptions, onTryAgain);
                    return;
                }
                OpponentBattleData = response2.BattleData;
            }
            EndPrepareTurn();
            ChangeToBattleState();
            
            return;
            void OnTryAgain()
            {
                TryEndPrepareTurn().Forget();
            }
        }
        private void EndPrepareTurn()
        {
            foreach (StoreSlot storeSlot in StoreSlotArray)
            {
                storeSlot.ClearStoreSlot();
            }
            Debug.Log("End Turn");
        }
        public void ReRollStoreSlots()
        {
            foreach (StoreSlot storeSlot in StoreSlotArray)
            {
                storeSlot.ReRollHeroFromPool();
            }
        }
        public void ChangeToBattleState()
        {
            StateMachine.RequestTransition(InGameBattleState.Instance);
        }
        public void ChangeToPrepareState()
        {
            StateMachine.RequestTransition(InGamePrepareState.Instance);
        }
        public void ClearAllFieldSlot()
        {
            foreach (FieldSlot fieldSlot in FieldSlotArray)
            {
                fieldSlot.ClearFieldSlot();
            }
        }
        public void ClearAllBattleSlot()
        {
            PlayerTeam.ClearAllBattleSlot();
            OpponentTeam.ClearAllBattleSlot();
        }

        public void RemoveHero(Hero hero)
        {
            PlayerTeam.TryRemoveHero(hero);
            OpponentTeam.TryRemoveHero(hero);
        }
        
        public InGameTeamData GetInGameTeamData()
        {
            return new InGameTeamData(FieldSlotArray);
        }

        private void ForceMoveHeroToEmptyField(Holder fromHeroHolder, Holder toHeroHolder, Hero hero)
        {
            if (toHeroHolder.HasHero) return;
            toHeroHolder.ForceAddHero(hero);
            fromHeroHolder.RemoveHero();
        }

        public void TryEmptyFieldSlot(FieldSlot fieldSlot)
        {
            Mark = new List<FieldSlot>() { null };
            Pool = new List<FieldSlot>() { fieldSlot };
            Solution = new List<FieldSlot>();
            FindSolution(0, Pool, Mark, Solution);
            if (Solution.Count == 0) return;
            foreach (FieldSlot heroSlot in Solution)
            {
                if (heroSlot == InputManager.CurrentHolderDrag)
                {
                    InputManager.SetCurrentHolderDrag(fieldSlot);
                }
            }

            for (int i = 0; i < Solution.Count - 1; i++)
            {
                ForceMoveHeroToEmptyField(Solution[i + 1], Solution[i], Solution[i + 1].OwnerHero);
            }
        }

        
        private void FindSolution(int currentStep, List<FieldSlot> pool, List<FieldSlot> mark,
            List<FieldSlot> solution)
        {
            if (currentStep >= pool.Count)
            {
                return;
            }

            FieldSlot fieldSlot = pool[currentStep];
            if (!fieldSlot.HasHero)
            {
                GetSolution(pool, mark, solution, fieldSlot);
                return;
            }

            foreach (FieldSlot slotAround in fieldSlot.FieldSlotAround)
            {
                if (pool.Contains(slotAround)) continue;
                pool.Add(slotAround);
                mark.Add(fieldSlot);
            }

            FindSolution(currentStep + 1, pool, mark, solution);
        }

        private static void GetSolution(List<FieldSlot> pool, List<FieldSlot> mark, List<FieldSlot> solution,
            FieldSlot fieldSlot)
        {
            solution.Add(fieldSlot);
            while (true)
            {
                int index = pool.IndexOf(solution[^1]);
                if (index == 0) break;
                solution.Add(mark[index]);
            }
        }
        
        public void TryTriggerAllyAbility<T>(Hero hero, TickRate tickRate, CancellationToken cancellationToken) where T : AbilityTrigger
        {
            if (!AllAllyTarget.TryFindTarget(hero, out Holder[] holders, out int count)) return;
            for (int i = 0; i < count; i++)
            {
                if (!holders[i].HasHero) continue;
                holders[i].OwnerHero.TryTriggerAbility<T>(tickRate, cancellationToken);
            }
        }
        public void TryTriggerAllySummonedAbility<T>(Hero hero, Hero summonedHero, TickRate tickRate, CancellationToken cancellationToken) where T : AbilityTrigger
        {
            if (!AllAllyTarget.TryFindTarget(hero, out Holder[] holders, out int count)) return;
            for (int i = 0; i < count; i++)
            {
                if (!holders[i].HasHero) continue;
                if (!holders[i].OwnerHero.HasTriggerEffect<T>()) continue;
                AbilityTarget abilityTarget = holders[i].OwnerHero.HeroConfigData.GetAbilityPower(holders[i].OwnerHero.GetStar()).AbilityTarget;
                if (abilityTarget is SummonedTarget summonedTarget)
                {
                    summonedTarget.TargetQueue.Enqueue(summonedHero);
                }
                holders[i].OwnerHero.TryTriggerAbility<T>(tickRate, cancellationToken);
            }
        }
    }
}