using System;
using Cysharp.Threading.Tasks;
using Game.Data;
using TW.Utility.DesignPattern;
using UnityEngine;
using Zenject;

namespace Game.Manager
{
    public class GameManager : Singleton<GameManager>
    {
        [Inject] private InGameHandleManager InGameHandleManager { get; set; }
        [Inject] private DatabaseManager DatabaseManager { get; set; }
        [Inject] private InGameDataManager InGameDataManager { get; set; }
        

        public void StartGame()
        {
            InGameHandleManager.CreateNewGame();
        }
        public void EndTurn()
        {
            // BattleManager.SetActive(true);
            // FieldSlotManager.SetActive(false);
            // InGameTeamData playerTeamData = FieldSlotManager.GetInGameTeamData();
            // InGameTeamData enemyTeamData = FieldSlotManager.GetInGameTeamData();
            // BattleManager.InitBattleField(playerTeamData, enemyTeamData);
            //
            // DatabaseManager.SaveBattleDataAsync(
            //     new BattleData(InGameDataManager.UserData.Rank, 0, 0, 1, playerTeamData))
            //     .Forget();
        }
    }
}