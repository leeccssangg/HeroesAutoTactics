using System;
using System.Collections.Generic;
using System.Text;
using Game.Core;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Data
{
    [Serializable]

    public class InGameTeamData
    {
        [field: SerializeField] public int TeamSeed { get; private set; }
        [field: SerializeField] public InGameHeroData[] InGameHeroDataArray { get; private set; }
        public static InGameTeamData Empty => new InGameTeamData().EmptyTeam();

        public InGameTeamData()
        {

        }
        public InGameTeamData EmptyTeam()
        {
            TeamSeed = Random.Range(0, 10000);
            InGameHeroDataArray = new InGameHeroData[6];
            for (int index = 0; index < InGameHeroDataArray.Length; index++)
            {
                InGameHeroDataArray[index] = new InGameHeroData();
            }

            return this;
        }
        public InGameTeamData(string data)
        {
            FromStringData(data);
        }
        public InGameTeamData(FieldSlot[] fieldSlotList)
        {
            TeamSeed = Random.Range(0, 10000);
            InGameHeroDataArray = new InGameHeroData[fieldSlotList.Length];

            for (int i = 0; i < fieldSlotList.Length; i++)
            {
                InGameHeroDataArray[i] = new InGameHeroData(fieldSlotList[i]);
            }
        }

        public string ToStringData()
        {
            StringBuilder data = new StringBuilder();
            data.Append(TeamSeed.ToString());
            data.Append("||");
            foreach (InGameHeroData inGameHeroData in InGameHeroDataArray)
            {
                data.Append(inGameHeroData.ToStringData());
                data.Append("|");
            }

            data.Remove(data.Length - 1, 1);
            return data.ToString();
        }

        public void FromStringData(string data)
        {
            string[] splitData = data.Split("||");
            TeamSeed = int.Parse(splitData[0]);
            string[] heroData = splitData[1].Split('|');
            InGameHeroDataArray = new InGameHeroData[heroData.Length];

            for (int i = 0; i < heroData.Length; i++)
            {
                InGameHeroDataArray[i] = new InGameHeroData(heroData[i]);
            }
        }
    }
}
