using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
        
namespace Game.GlobalConfig
{

    [CreateAssetMenu(fileName = "HeroUpgradeGlobalConfig", menuName = "GlobalConfigs/HeroUpgradeGlobalConfig")]
    [GlobalConfig("Assets/Resources/GlobalConfig/")]
    public class HeroUpgradeGlobalConfig : GlobalConfig<HeroUpgradeGlobalConfig>
    {
        public List<HeroUpgradeConfig> HeroUpgradeConfigs = new();

        public HeroUpgradeConfig GetHeroUpgradeConfig(Combat.Rarity rarity)
        {
            //return HeroUpgradeConfigs.Find(x => x.rarity == rarity);
            for (int i = 0; i < HeroUpgradeConfigs.Count; i++)
            {
                if (HeroUpgradeConfigs[i].rarity == rarity)
                {
                    return HeroUpgradeConfigs[i];
                }
            }
            return null;
        }
        public HeroUpgradeLevelConfig GetHeroUpgradeLevelConfig(Combat.Rarity rarity, int level)
        {
            HeroUpgradeConfig heroUpgradeConfig = GetHeroUpgradeConfig(rarity);
            //return heroUpgradeConfig.Levels.Find(x => x.level == level);
            for (int i = 0; i < heroUpgradeConfig.Levels.Count; i++)
            {
                if (heroUpgradeConfig.Levels[i].level == level)
                {
                    return heroUpgradeConfig.Levels[i];
                }
            }
            return null;
        }
    }
    [System.Serializable]
    public class HeroUpgradeConfig
    {
        public Combat.Rarity rarity;
        public List<HeroUpgradeLevelConfig> Levels;

        public int GetPieceCostUpgrade(int level)
        {
            //return Levels.Find(x => x.level == level).pieceCost;
            for(int i = 0; i < Levels.Count; i++)
            {
                if (Levels[i].level == level)
                {
                    return Levels[i].pieceCost;
                }
            }   
            return 0;
        }
    }
    [System.Serializable]
    public class HeroUpgradeLevelConfig
    {
        public int level;
        public int pieceCost;
        //TODO: Add resource cost
    }
}