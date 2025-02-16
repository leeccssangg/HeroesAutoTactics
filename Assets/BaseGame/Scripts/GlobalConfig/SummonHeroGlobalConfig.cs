using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using TW.Utility.CustomType;
using Sirenix.OdinInspector;
using Combat;
using Game.Manager;
using Game.Data;

namespace Game.GlobalConfig
{
    public enum SummonHeroType
    {
        None,
        HeroCommon,
        HeroUncommon,
        HeroRare,
        HeroEpic,
        HeroLegendary,
        HeroMythic,
        Gold,
        Diamond,
        MythicStone,
    }
    [CreateAssetMenu(fileName = "SummonHeroGlobalConfig", menuName = "GlobalConfigs/SummonHeroGlobalConfig")]
    [GlobalConfig("Assets/Resources/GlobalConfig/")]
    public class SummonHeroGlobalConfig : GlobalConfig<SummonHeroGlobalConfig>
    {
        [field: SerializeField] public Probability<int> NumAppearanceProbability { get; private set; } = new(new());
        [field: SerializeField] public Probability<SummonHeroConfig> SummonHeroConfigProbability { get; private set; } = new(new());
        [field: SerializeField] public int StartStage { get; private set; }
    }

    [System.Serializable]
    public class SummonHeroConfig
    {
        [field: SerializeField] public SummonHeroType Type { get; private set; }
        [field: SerializeField] public int RewardAmount { get; private set; }
        [MaxValue(100)]
        [field: SerializeField] public int acquisitionRate { get; private set; }

        public bool IsAcquired()
        {
            return Random.Range(0, 100) <= acquisitionRate;
        }
    }
    [System.Serializable]
    public class SummonHeroReward
    {
        //[field: SerializeField] public SummonHeroType Type { get; private set; }
        //[field: SerializeField] public int RewardAmount { get; private set; }
        [field: SerializeField] public SummonHeroConfig Config { get; private set; }
        [field: SerializeField] public bool IsAcquired { get; private set; }
        [field: SerializeField] public int NumGacha { get; private set; }
        [field: SerializeField] public Family Synergy { get; private set; }
        [field: SerializeField] public HeroConfigData RewardHero { get;private set; }
        //TODO Add gameresource config

        public SummonHeroReward(SummonHeroConfig summonHeroConfig, bool isAcquired, int numGacha, Family family)
        {
            Config = summonHeroConfig;
            IsAcquired = isAcquired;
            NumGacha = numGacha;
            if(Config.Type != SummonHeroType.Gold
                && Config.Type != SummonHeroType.Diamond
                && Config.Type != SummonHeroType.MythicStone
                )
            {
                Synergy = family;
            }
            else
            {
                Synergy = Family.None;
            }
            GetRandomHeroConfigData();
            GetResourceReward();
        }
        private void GetRandomHeroConfigData()
        {
            if (Synergy == Family.None) return;
            switch (Config.Type)
            {
                case SummonHeroType.HeroCommon:
                    RewardHero = HeroManager.Instance.GetRandomHeroConfigDataByRarityAndFamily(Rarity.Common, Synergy);
                    break;
                case SummonHeroType.HeroUncommon:
                    RewardHero = HeroManager.Instance.GetRandomHeroConfigDataByRarityAndFamily(Rarity.Uncommon, Synergy);
                    break;
                case SummonHeroType.HeroRare:
                    RewardHero = HeroManager.Instance.GetRandomHeroConfigDataByRarityAndFamily(Rarity.Rare, Synergy);
                    break;
                case SummonHeroType.HeroEpic:
                    RewardHero = HeroManager.Instance.GetRandomHeroConfigDataByRarityAndFamily(Rarity.Epic, Synergy);
                    break;
                case SummonHeroType.HeroLegendary:
                    RewardHero = HeroManager.Instance.GetRandomHeroConfigDataByRarityAndFamily(Rarity.Legendary, Synergy);
                    break;
                case SummonHeroType.HeroMythic:
                    RewardHero = HeroManager.Instance.GetRandomHeroConfigDataByRarityAndFamily(Rarity.Mythic, Synergy);
                    break;
                default:
                    break;
            }
        }
        private void GetResourceReward()
        {
            if (Synergy != Family.None) return;
            //TODO Add resource reward;
        }
        public void ClaimReward()
        {
            if(Synergy != Family.None)
            {
                EachHeroUpgradeData heroUpgradeData = HeroManager.Instance.GetEachHeroUpgradeData(RewardHero.HeroId);
                HeroManager.Instance.AddHeroPiece(heroUpgradeData.HeroId, Config.RewardAmount*NumGacha);
            }
            else
            {
                //TODO Claim Resources
            }
        }
    }
}