using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Core;
using Sirenix.OdinInspector;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;
using Random = System.Random;

namespace Game.Manager
{
    public delegate List<Holder> TargetingFindingFunction(Hero hero, Effect effect);

    public class TargetingManager : Singleton<TargetingManager>
    {
        // [Inject] private InGameHandleManager InGameHandleManager { get; set; }
        // private Dictionary<Targeting, TargetingFindingFunction> TargetingDictionary { get; set; } = new();
        // private List<Holder> EmptyHolders { get; set; } = new List<Holder>();
        //
        // private void Start()
        // {
        //     TargetingDictionary.Add(Targeting.RandomOpponent, GetRandomOpponent);
        //     TargetingDictionary.Add(Targeting.AllOpponent, GetAllAlly);
        //     TargetingDictionary.Add(Targeting.Self, GetSelf);
        //     TargetingDictionary.Add(Targeting.RandomAlly, GetRandomAlly);
        //     TargetingDictionary.Add(Targeting.AllAlly, GetAllAlly);
        //     TargetingDictionary.Add(Targeting.Everyone, GetEveryone);
        //     TargetingDictionary.Add(Targeting.AllAllyAndSelf, GetAllAllyAndSelf);
        //     TargetingDictionary.Add(Targeting.AllStoreHero, GetAllStoreHero);
        //     TargetingDictionary.Add(Targeting.RandomStoreHeroWithFaintPassive, GetRandomStoreHeroWithFaintPassive);
        //     TargetingDictionary.Add(Targeting.FirstOpponent, GetFirstOpponent);
        //     TargetingDictionary.Add(Targeting.NearHeroAHead, GetNearHeroAHead);
        //     TargetingDictionary.Add(Targeting.NearHeroBehind, GetNearHeroBehind);
        //     
        // }
        //
        //
        // Random Random => InGameHandleManager.Random;
        //
        // public List<Holder> GetTarget(Hero hero, Effect effect)
        // {
        //     return TargetingDictionary.TryGetValue(effect.Targeting, out TargetingFindingFunction func)
        //         ? func(hero, effect)
        //         : EmptyHolders;
        // }
        //
        // public List<Holder> GetAllAlly(Hero hero, Effect effect)
        // {
        //     if (InGameHandleManager.IsInPrepareState())
        //     {
        //         return GetAllyHoldersInPrepareState(hero);
        //     }
        //
        //     if (InGameHandleManager.IsInBattleState())
        //     {
        //         return GetAllyHoldersInBattleState(hero);
        //     }
        //
        //     Debug.Log("Not found hero in team");
        //     return EmptyHolders;
        // }
        //
        // public List<Holder> GetAllOpponent(Hero hero, Effect effect)
        // {
        //     if (InGameHandleManager.IsInBattleState())
        //     {
        //         return GetOpponentHoldersInBattleState(hero);
        //     }
        //
        //     Debug.Log("Not found hero in team");
        //     return EmptyHolders;
        // }
        //
        // private List<Holder> GetAllyHoldersInPrepareState(Hero hero)
        // {
        //     List<Holder> resultHolders = new List<Holder>();
        //     foreach (FieldSlot fieldSlot in InGameHandleManager.FieldSlotArray)
        //     {
        //         if (fieldSlot.HasHero && fieldSlot.OwnerHero != hero)
        //         {
        //             resultHolders.Add(fieldSlot);
        //         }
        //     }
        //
        //     return resultHolders;
        // }
        //
        // private List<Holder> GetAllyHoldersInBattleState(Hero hero)
        // {
        //     if (TryFindHeroInTeam(InGameHandleManager.PlayerTeam, out List<Holder> playerHolders))
        //     {
        //         return playerHolders;
        //     }
        //
        //     if (TryFindHeroInTeam(InGameHandleManager.OpponentTeam, out List<Holder> opponentHolders))
        //     {
        //         return opponentHolders;
        //     }
        //
        //     return EmptyHolders;
        //
        //     bool TryFindHeroInTeam(BattleTeam battleTeam, out List<Holder> holders)
        //     {
        //         holders = new List<Holder>();
        //         bool isFind = false;
        //         foreach (BattleSlot battleSlot in battleTeam.BattleSlotArray)
        //         {
        //             if (battleSlot == hero.LastHolder)
        //             {
        //                 isFind = true;
        //             }
        //             if (!battleSlot.HasHero) continue;
        //             holders.Add(battleSlot);
        //             if (battleSlot.OwnerHero != hero) continue;
        //             holders.Remove(battleSlot);
        //             
        //         }
        //
        //         return isFind;
        //     }
        // }
        //
        // private List<Holder> GetOpponentHoldersInBattleState(Hero hero)
        // {
        //     bool isInPlayerTeam = TryFindHeroInTeam(InGameHandleManager.PlayerTeam, out List<Holder> playerHolders);
        //     bool isInOpponentTeam =
        //         TryFindHeroInTeam(InGameHandleManager.OpponentTeam, out List<Holder> opponentHolders);
        //     if (isInPlayerTeam)
        //     {
        //         return opponentHolders;
        //     }
        //
        //     if (isInOpponentTeam)
        //     {
        //         return playerHolders;
        //     }
        //
        //     return EmptyHolders;
        //
        //     bool TryFindHeroInTeam(BattleTeam battleTeam, out List<Holder> holders)
        //     {
        //         holders = new List<Holder>();
        //         bool isFind = false;
        //         foreach (BattleSlot battleSlot in battleTeam.BattleSlotArray)
        //         {
        //             if (battleSlot == hero.LastHolder)
        //             {
        //                 isFind = true;
        //             }
        //             if (!battleSlot.HasHero) continue;
        //             holders.Add(battleSlot);
        //         }
        //
        //         return isFind;
        //     }
        // }
        //
        //
        // private List<Holder> GetSelf(Hero hero, Effect effect)
        // {
        //     if (InGameHandleManager.IsInPrepareState())
        //     {
        //         return GetSelfInPrepareState(hero);
        //     }
        //
        //     if (InGameHandleManager.IsInBattleState())
        //     {
        //         return GetSelfInBattleState(hero);
        //     }
        //
        //     Debug.Log("Not found hero in team");
        //     return EmptyHolders;
        // }
        //
        // private List<Holder> GetSelfInPrepareState(Hero hero)
        // {
        //     List<Holder> resultHolders = new List<Holder>();
        //     foreach (FieldSlot fieldSlot in InGameHandleManager.FieldSlotArray)
        //     {
        //         if (fieldSlot.HasHero && fieldSlot.OwnerHero == hero)
        //         {
        //             resultHolders.Add(fieldSlot);
        //         }
        //     }
        //
        //     return resultHolders;
        // }
        //
        // private List<Holder> GetSelfInBattleState(Hero hero)
        // {
        //     if (TryFindHeroInTeam(InGameHandleManager.PlayerTeam, out List<Holder> playerHolders))
        //     {
        //         return playerHolders;
        //     }
        //
        //     if (TryFindHeroInTeam(InGameHandleManager.OpponentTeam, out List<Holder> opponentHolders))
        //     {
        //         return opponentHolders;
        //     }
        //
        //     return EmptyHolders;
        //
        //     bool TryFindHeroInTeam(BattleTeam battleTeam, out List<Holder> holders)
        //     {
        //         holders = new List<Holder>();
        //         bool isFind = false;
        //         foreach (BattleSlot battleSlot in battleTeam.BattleSlotArray)
        //         {
        //             if (!battleSlot.HasHero || battleSlot.OwnerHero != hero) continue;
        //             holders.Add(battleSlot);
        //             isFind = true;
        //         }
        //
        //         return isFind;
        //     }
        // }
        //
        // private List<Holder> GetRandomOpponent(Hero hero, Effect effect)
        // {
        //     List<Holder> enemies = GetAllOpponent(hero, effect);
        //     if (enemies.Count == 0) return EmptyHolders;
        //     TargetValue targetValue = effect.GetTargetValue(hero.GetLevel());
        //     int count = Mathf.Min(targetValue.Value0, enemies.Count);
        //     enemies.ShuffleList(Random);
        //     return enemies.GetRange(0, count);
        // }
        //
        // private List<Holder> GetRandomAlly(Hero hero, Effect effect)
        // {
        //     List<Holder> allies = GetAllAlly(hero, effect);
        //     if (allies.Count == 0) return EmptyHolders;
        //     TargetValue targetValue = effect.GetTargetValue(hero.GetLevel());
        //     int count = Mathf.Min(targetValue.Value0, allies.Count);
        //     allies.ShuffleList(Random);
        //     return allies.GetRange(0, count);
        // }
        //
        // private List<Holder> GetEveryone(Hero hero, Effect effect)
        // {
        //     List<Holder> resultHolders = new List<Holder>();
        //     resultHolders.AddRange(GetAllAlly(hero, effect));
        //     resultHolders.AddRange(GetAllOpponent(hero, effect));
        //     resultHolders.AddRange(GetSelf(hero, effect));
        //     return resultHolders;
        // }
        //
        //
        // private List<Holder> GetAllAllyAndSelf(Hero hero, Effect effect)
        // {
        //     List<Holder> resultHolders = new List<Holder>();
        //     resultHolders.AddRange(GetAllAlly(hero, effect));
        //     resultHolders.AddRange(GetSelf(hero, effect));
        //     return resultHolders;
        // }
        //
        // private List<Holder> GetAllStoreHero(Hero hero, Effect effect)
        // {
        //     List<Holder> resultHolders = new List<Holder>();
        //     foreach (StoreSlot storeSlot in InGameHandleManager.StoreSlotArray)
        //     {
        //         if (storeSlot.HasHero)
        //         {
        //             resultHolders.Add(storeSlot);
        //         }
        //     }
        //
        //     return resultHolders;
        // }
        // private List<Holder> GetFirstOpponent(Hero hero, Effect effect)
        // {
        //     List<Holder> enemies = GetAllOpponent(hero, effect);
        //     if (enemies.Count == 0) return EmptyHolders;
        //     return new List<Holder> {enemies[0]};
        // }
        // private List<Holder> GetNearHeroAHead(Hero hero, Effect effect)
        // {
        //     List<Holder> allies = GetAllAlly(hero, effect);
        //     if (hero.LastHolder == allies[0])
        //     {
        //         List<Holder> opponent = GetAllOpponent(hero, effect);
        //         if (opponent.Count == 0) return EmptyHolders;
        //         return new List<Holder> {opponent[0]};
        //     }
        //     if (allies.Count == 0) return EmptyHolders;
        //     for (int i = 1; i < allies.Count; i++)
        //     {
        //         if (hero.LastHolder == allies[0])
        //         {
        //             return new List<Holder> {allies[i-1]};
        //         }
        //     }
        //     return EmptyHolders;
        // }
        // private List<Holder> GetNearHeroBehind(Hero hero, Effect effect)
        // {
        //     List<Holder> allies = GetAllAlly(hero, effect);
        //     if (allies.Count == 0) return EmptyHolders;
        //     for (int i = 0; i < allies.Count-1; i++)
        //     {
        //         if (hero.LastHolder == allies[i])
        //         {
        //             return new List<Holder> {allies[i+1]};
        //         }
        //     }
        //     return EmptyHolders;
        //     
        // }
        //
        // private List<Holder> GetRandomStoreHeroWithFaintPassive(Hero hero, Effect effect)
        // {
        //     List<Holder> allies = GetAllAlly(hero, effect);
        //     if (allies.Count == 0) return EmptyHolders;
        //     TargetValue targetValue = effect.GetTargetValue(hero.GetLevel());
        //     int count = Mathf.Min(targetValue.Value0, allies.Count);
        //     allies.ShuffleList(Random);
        //     return allies.GetRange(0, count);
        // }

        // public async UniTask<List<Holder>> GetEmptyHolderToSummon(Hero hero, int holderNeed)
        // {
        //     if (InGameHandleManager.IsInPrepareState())
        //     {
        //         return await GetEmptyHolderToSummonInPrepareState(hero, holderNeed);
        //     }
        //
        //     if (InGameHandleManager.IsInBattleState())
        //     {
        //         return await GetEmptyHolderToSummonInBattleState(hero, holderNeed);
        //     }
        //
        //     return EmptyHolders;
        // }
        //
        // private async UniTask<List<Holder>> GetEmptyHolderToSummonInPrepareState(Hero hero, int holderNeed)
        // {
        //     Holder[] findHolder = InGameHandleManager.FieldSlotArray.Cast<Holder>().ToArray();
        //     return await InGameHandleManager.TryEmptyHolder(findHolder, hero.LastHolder, holderNeed, TickRate.Normal,
        //         this.GetCancellationTokenOnDestroy());
        // }
        //
        // private async UniTask<List<Holder>> GetEmptyHolderToSummonInBattleState(Hero hero, int holderNeed)
        // {
        //     if (TryFindEmptyHolderInTeam(InGameHandleManager.PlayerTeam, out Holder[] playerHolders))
        //     {
        //         return await InGameHandleManager.TryEmptyHolder(playerHolders, hero.LastHolder, holderNeed,
        //             TickRate.Normal, this.GetCancellationTokenOnDestroy());
        //         ;
        //     }
        //
        //     if (TryFindEmptyHolderInTeam(InGameHandleManager.OpponentTeam, out Holder[] opponentHolders))
        //     {
        //         return await InGameHandleManager.TryEmptyHolder(opponentHolders, hero.LastHolder, holderNeed,
        //             TickRate.Normal, this.GetCancellationTokenOnDestroy());
        //         ;
        //     }
        //
        //     return EmptyHolders;
        //
        //     bool TryFindEmptyHolderInTeam(BattleTeam battleTeam, out Holder[] holders)
        //     {
        //         holders = battleTeam.BattleSlotArray.Cast<Holder>().ToArray();
        //         foreach (BattleSlot battleSlot in battleTeam.BattleSlotArray)
        //         {
        //             if (hero.LastHolder == battleSlot)
        //             {
        //                 return true;
        //             }
        //         }
        //
        //         return false;
        //     }
        // }

    }
}