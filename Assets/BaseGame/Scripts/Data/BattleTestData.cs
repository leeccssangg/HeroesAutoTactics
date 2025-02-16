using System;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class BattleTestData
    {
        [field: SerializeField] public int Heart {get; private set;}
        [field: SerializeField] public int Round {get; private set;}
        [field: SerializeField] public int Win {get; private set;}
        [field: SerializeField] public BattleData PlayerData {get; private set;}
        [field: SerializeField] public BattleData OpponentData {get; private set;}
        
        public BattleTestData(int heart, int round, int win, BattleData playerData, BattleData opponentData)
        {
            Heart = heart;
            Round = round;
            Win = win;
            PlayerData = playerData;
            OpponentData = opponentData;
        }

        public BattleTestData(string jsonData)
        {
            FromJsonData(jsonData);
        }
        
        public string ToJsonData()
        {
            return JsonUtility.ToJson(this);
        }
        
        public void FromJsonData(string jsonData)
        {
            JsonUtility.FromJsonOverwrite(jsonData, this);
        }
        
    }
}