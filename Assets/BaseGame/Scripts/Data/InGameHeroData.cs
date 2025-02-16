using Game.Core;
using UnityEngine;

namespace Game.Data
{
    [System.Serializable]
    public class InGameHeroData
    {
        [field: SerializeField] public int HeroId {get; set;}
        [field: SerializeField] public int Pieces {get; set;}
        [field: SerializeField] public int AttackDamageBuff {get; set;}
        [field: SerializeField] public int HealthPointBuff {get; set;}
    
        public string ToStringData()
        {
            return $"{HeroId.ToString()}:{Pieces.ToString()}:{AttackDamageBuff.ToString()}:{HealthPointBuff.ToString()}";
        }
        public void FromStringData(string data)
        {
            string[] splitData = data.Split(':');
            HeroId = int.Parse(splitData[0]);
            Pieces = int.Parse(splitData[1]);
            AttackDamageBuff = int.Parse(splitData[2]);
            HealthPointBuff = int.Parse(splitData[3]);
        }
        public InGameHeroData()
        {
            HeroId = -1;
            Pieces = 0;
            AttackDamageBuff = 0;
            HealthPointBuff = 0;
        }

        public InGameHeroData(FieldSlot fieldHeroSlot)
        {
            if (!fieldHeroSlot.HasHero)
            {
                HeroId = -1;
                return;
            }

            HeroId = fieldHeroSlot.OwnerHero.HeroConfigData.HeroId;
            Pieces = fieldHeroSlot.OwnerHero.Pieces;
            AttackDamageBuff = fieldHeroSlot.OwnerHero.AttackDamageBuff;
            HealthPointBuff = fieldHeroSlot.OwnerHero.HealthPointBuff;
        }
        public InGameHeroData(string data)
        {
            string[] splitData = data.Split(':');
            HeroId = int.Parse(splitData[0]);
            Pieces = int.Parse(splitData[1]);
            AttackDamageBuff = int.Parse(splitData[2]);
            HealthPointBuff = int.Parse(splitData[3]);
        }

    }
}