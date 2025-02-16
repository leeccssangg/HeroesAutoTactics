using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Manager;
using UnityEngine;
using Random = System.Random;

namespace Combat
{
    public abstract class AbilityTarget : ScriptableObject
    {
        protected InGameHandleManager InGameHandleManager {get; set;}
        protected Holder[] FieldSlots {get; set;}
        protected Holder[] StoreSlots {get; set;}
        protected Holder[] BattleSlot1 {get; set;}
        protected Holder[] BattleSlot2 {get; set;}
        public AbilityTarget Initialize(InGameHandleManager inGameHandleManager)
        {
            InGameHandleManager = inGameHandleManager;
            
            FieldSlots = InGameHandleManager.FieldSlotArray.ToHolder();
            StoreSlots = InGameHandleManager.StoreSlotArray.ToHolder();
            BattleSlot1 = InGameHandleManager.PlayerTeam.BattleSlotArray.ToHolder();
            BattleSlot2 = InGameHandleManager.OpponentTeam.BattleSlotArray.ToHolder();
            
            return this;
        }
        protected bool HasHolder(Holder[] holders, Holder holder)
        {
            foreach (Holder hol in holders)
            {
                if (hol == holder)
                {
                    return true;
                }
            }
            return false;
        }
        public Holder[] GetAllyBattleSlot(Hero hero)
        {
            if (HasHolder(BattleSlot1, hero.LastHolder))
            {
                return BattleSlot1;
            }
            if (HasHolder(BattleSlot2, hero.LastHolder))
            {
                return BattleSlot2;
            }
            return null;
        }
        public Holder[] GetOpponentBattleSlot(Hero hero)
        {
            if (HasHolder(BattleSlot1, hero.LastHolder))
            {
                return BattleSlot2;
            }
            if (HasHolder(BattleSlot2, hero.LastHolder))
            {
                return BattleSlot1;
            }
            return null;
        }
        
        public async UniTask<Holder[]> GetEmptyHolderToSummon(Hero hero, int holderNeed, TickRate tickRate, CancellationToken cancellationToken)
        {
            if (InGameHandleManager.IsInPrepareState())
            {
                return await GetEmptyHolderToSummonInPrepareState(hero, holderNeed, tickRate, cancellationToken);
            }

            if (InGameHandleManager.IsInBattleState())
            {
                return await GetEmptyHolderToSummonInBattleState(hero, holderNeed, tickRate, cancellationToken);
            }

            return Array.Empty<Holder>();
        }
        private async UniTask<Holder[]> GetEmptyHolderToSummonInPrepareState(Hero hero, int holderNeed, TickRate tickRate, CancellationToken cancellationToken)
        {
            Holder[] findHolder = InGameHandleManager.FieldSlotArray.Cast<Holder>().ToArray();
            return await TryEmptyHolder(findHolder, hero.LastHolder, holderNeed, tickRate, cancellationToken);
        }
        private async UniTask<Holder[]> GetEmptyHolderToSummonInBattleState(Hero hero, int holderNeed, TickRate tickRate, CancellationToken cancellationToken)
        {
            if (TryFindEmptyHolderInTeam(InGameHandleManager.PlayerTeam, out Holder[] playerHolders))
            {
                return await TryEmptyHolder(playerHolders, hero.LastHolder, holderNeed, tickRate, cancellationToken);
            }

            if (TryFindEmptyHolderInTeam(InGameHandleManager.OpponentTeam, out Holder[] opponentHolders))
            {
                return await TryEmptyHolder(opponentHolders, hero.LastHolder, holderNeed, tickRate, cancellationToken);
            }

            return Array.Empty<Holder>();

            bool TryFindEmptyHolderInTeam(BattleTeam battleTeam, out Holder[] holders)
            {
                holders = battleTeam.BattleSlotArray.Cast<Holder>().ToArray();
                foreach (BattleSlot battleSlot in battleTeam.BattleSlotArray)
                {
                    if (hero.LastHolder == battleSlot)
                    {
                        return true;
                    }
                }

                return false;
            }
        }
        public async UniTask<Holder[]> TryEmptyHolder(Holder[] holderArray, Holder pointHolder, int space, TickRate tickRate, CancellationToken cancellationToken)
        {
            int pointIndex = Array.IndexOf(holderArray, pointHolder);
            int emptyCount = 0;
            int startPoint = pointIndex;
            int endPoint = pointIndex;
            List<Hero> startHeroList = holderArray.Select(t => t.HasHero ? t.OwnerHero : null).ToList();
            List<Hero> endHeroList = holderArray.Select(t => t.HasHero ? t.OwnerHero : null).ToList();
            for (int i = pointIndex; i < startHeroList.Count; i++)
            {
                if (emptyCount >= space)
                {
                    break;
                }

                if (endHeroList[i] != null) continue;
                endHeroList.RemoveAt(i);
                endHeroList.Insert(pointIndex, null);
                emptyCount++;
                endPoint++;
            }

            for (int i = pointIndex -1; i > -1 ; i--)
            {
                if (emptyCount >= space)
                {
                    break;
                }

                if (endHeroList[i] != null) continue;
                endHeroList.RemoveAt(i);
                endHeroList.Insert(pointIndex, null);
                emptyCount++;
                startPoint--;
            }

            for (int i = 0; i < startHeroList.Count; i++)
            {
                if (startHeroList[i]== null) continue;
                holderArray[i].RemoveHero();
            }
            List<UniTask> taskList = new List<UniTask>();
            for (int i = 0; i < startHeroList.Count; i++)
            {
                if (startHeroList[i]== null) continue;
                taskList.Add(ForceMoveHeroToEmptyField(holderArray, i, endHeroList.IndexOf(startHeroList[i]), startHeroList[i], tickRate, cancellationToken));
            }
            await UniTask.WhenAll(taskList);
            return holderArray[startPoint..endPoint];
        }
        private async UniTask ForceMoveHeroToEmptyField(Holder[] holderPoint, int from, int to,  Hero hero, TickRate tickRate, CancellationToken cancellationToken)
        {
            if (from == to)
            {
                holderPoint[to].ForceAddHeroInstance(hero);
            }
            Holder[] holderArray = new Holder[Math.Abs(from - to) + 1];
            for (int i = 0; i < holderArray.Length; i++)
            {
                holderArray[i] = holderPoint[(from < to )? (from + i ): (from - i)];
            }
            await holderArray[^1].ForceAddHero(holderArray, hero, tickRate, cancellationToken);
        }
        public abstract bool TryFindTarget(Hero hero, out Holder[] holders, out int count);
    }
}