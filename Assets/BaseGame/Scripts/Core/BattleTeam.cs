using System.Collections.Generic;
using System.Threading;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Data;
using TW.Utility.Extension;
using UnityEngine;

namespace Game.Core
{
    public class BattleTeam : MonoBehaviour
    {
        [field: SerializeField] public InGameTeamData InGameTeamData { get; private set; }
        [field: SerializeField] public BattleSlot[] BattleSlotArray { get; private set; }
        [field: SerializeField] public int CurrentPosition { get; private set; }

        public void InitBattleTeam(InGameTeamData inGameTeamData)
        {
            InGameTeamData = inGameTeamData;
            CurrentPosition = 0;
            for (int i = 1; i < BattleSlotArray.Length; i++)
            {
                BattleSlotArray[i].InitBattleSlot(InGameTeamData.InGameHeroDataArray[i-1]);
            }
        }
        public bool TryGetFirstHero(out Hero hero)
        {
            hero = BattleSlotArray[0].OwnerHero;
            return BattleSlotArray[0].HasHero;
        }
        
        public bool IsAllFainted()
        {
            foreach (BattleSlot battleSlot in BattleSlotArray)
            {
                if (battleSlot.HasHero && !battleSlot.OwnerHero.IsFainted)
                {
                    return false;
                }
            }

            return true;
        }

        public void TryRemoveHero(Hero hero)
        {
            foreach (BattleSlot battleSlot in BattleSlotArray)
            {
                if (battleSlot.HasHero && battleSlot.OwnerHero == hero)
                {
                    battleSlot.RemoveHero();
                }
            }
        }

        public void ClearAllBattleSlot()
        {
            foreach (BattleSlot battleSlot in BattleSlotArray)
            {
                battleSlot.ClearBattleSlot();
            }
        }

        public bool isLog;
        public async UniTask ReRangeHero(TickRate tickRate, CancellationToken cancellationToken)
        {
            int index = 0;
            List<UniTask> tasks = new List<UniTask>();
            List<Vector3> points = new List<Vector3>();
            for (int i = 0; i < BattleSlotArray.Length; i++)
            {
                points.Insert(0, BattleSlotArray[i].Transform.position);
                if (!BattleSlotArray[i].TryGetHero(out Hero hero)) continue; 
                if (index == i)
                {
                    index++;
                    points.RemoveAt(points.Count - 1);
                    continue;
                }
                tasks.Add(hero.MoveToBattleSlot(BattleSlotArray[i], BattleSlotArray[index], tickRate, cancellationToken, points.ToArray())); 
                if (isLog)
                {
                    Debug.Log("ReRangeHero: " + hero.name + " from " + i + " to " + index);
                }
                index++;
                points.RemoveAt(points.Count - 1);
            }
            await UniTask.WhenAll(tasks);
        }
    }
}