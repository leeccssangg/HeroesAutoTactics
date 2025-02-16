using System;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public class BattleData
    {
        [field: SerializeField] public Rank Rank { get; private set; }
        [field: SerializeField] public int Win { get; private set; }
        [field: SerializeField] public int Heart { get; private set; }
        [field: SerializeField] public int Round { get; private set; }
        
        [field: SerializeField] public InGameTeamData InGameTeamData {get; private set;}
        public string DataPath => $"{Win}-{Heart}-{Round}";

        public string TeamValue => InGameTeamData.ToStringData();

        public BattleData(Rank rank, int win, int heart, int round, InGameTeamData inGameTeamData)
        {
            Rank = rank;
            Win = win;
            Heart = heart;
            Round = round;
            InGameTeamData = inGameTeamData;
        }
    }
}