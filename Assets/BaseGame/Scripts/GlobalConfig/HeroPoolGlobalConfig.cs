using System.Collections.Generic;
using System.Linq;
using Game.Core;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using TW.Utility.Extension;
using UnityEngine;
using Zenject;
using Combat;
using Game.Manager;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Game.GlobalConfig
{
    [CreateAssetMenu(fileName = "HeroPoolGlobalConfig", menuName = "GlobalConfigs/HeroPoolGlobalConfig")]
    [GlobalConfig("Assets/Resources/GlobalConfig/")]
    public class HeroPoolGlobalConfig : GlobalConfig<HeroPoolGlobalConfig>
    {
        [System.Serializable]
        public class HeroFool
        {
            [field: SerializeField] public HeroConfigData[] HeroConfigDataArray {get; set;}
        }
        
        [field: SerializeField] private HeroConfigData[] TestConfig {get; set;}
        [field: SerializeField] public HeroConfigData[] HeroConfigDataArray { get; private set; }
        [field: SerializeField] public HeroFool[] HeroFoolArray {get; private set;}

        private Dictionary<int, HeroConfigData> m_HeroConfigDataDic;
        public Dictionary<int, HeroConfigData> HeroConfigDataDic => m_HeroConfigDataDic ??= GetHeroConfigDataDic();
        private Dictionary<int, HeroConfigData> GetHeroConfigDataDic()
        {
            return HeroConfigDataArray.ToDictionary(x => x.HeroId);
        }
        [Button]
        public void GetAllHeroConfigData()
        {
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
            HeroConfigDataArray = AssetDatabase.FindAssets("t:HeroConfigData")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<HeroConfigData>)
                .ToArray();
            
            Rarity[] rarityPool = new Rarity[] {
                Rarity.Common, 
                Rarity.Common, 
                Rarity.Uncommon,
                Rarity.Uncommon,
                Rarity.Rare, 
                Rarity.Rare, 
                Rarity.Epic, 
                Rarity.Epic, 
                Rarity.Legendary,
                Rarity.Legendary,
                Rarity.Mythic,
                Rarity.Mythic
            };
            HeroFoolArray = new HeroFool[10];
            for (int i = 0; i < 10; i++)
            {
                HeroFoolArray[i] = new HeroFool
                {
                    HeroConfigDataArray = HeroConfigDataArray.Where(c => c.Rarity <= rarityPool[i] && !c.IsToken).ToArray()
                };
            }
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
#endif
        }
        public HeroConfigData GetRandomHeroConfigData()
        {
            if (TestConfig is { Length: > 0 })
            {
                return TestConfig.GetRandomElement();
            }
            return HeroConfigDataArray.GetRandomElement();
        }
        public HeroConfigData GetRandomHeroConfigData(int round)
        {
            int poolIndex = Mathf.Clamp(round -1, 0, HeroFoolArray.Length - 1);
            return HeroFoolArray[poolIndex].HeroConfigDataArray.GetRandomElement();
        }
        public HeroConfigData GetRandomHeroConfigDataByRarityAndFamily(Rarity rarity, Family family)
        {
            return HeroConfigDataArray.Where(x => x.Rarity == rarity && x.HeroFamily == family).GetRandomElement();
        }

        public HeroConfigData GetHeroConfigData(int heroId)
        {
            return HeroConfigDataDic.GetValueOrDefault(heroId);
        }
        public List<HeroConfigData> GetListHeroConfigDataBySynergy(Family synergy)
        {
            return HeroConfigDataArray.Where(x => x.HeroFamily == synergy).ToList();
        }
        public int GetRandomFaintAlly(int star, System.Random random)
        {
            HeroConfigData[] shuffled = Shuffle(HeroConfigDataArray, random);
            foreach (HeroConfigData heroConfigData in shuffled)
            {
                if (heroConfigData.GetAbilityPower(star))
                {
                    return heroConfigData.HeroId;
                }
            }

            return -1;
        }

        private HeroConfigData[] Shuffle(HeroConfigData[] holders ,System.Random random)
        {
            int n = holders.Length;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (holders[k], holders[n]) = (holders[n], holders[k]);
            }
            return holders;
        }



    }
}