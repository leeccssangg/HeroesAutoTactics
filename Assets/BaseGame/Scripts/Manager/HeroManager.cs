using System;
using Cysharp.Threading.Tasks;
using Game.Data;
using TW.Utility.DesignPattern;
using UnityEngine;
using Sirenix.OdinInspector;
using Zenject;
using Game.GlobalConfig;
using static Game.GlobalConfig.SummonHeroGlobalConfig;
using System.Collections.Generic;
using TW.Reactive.CustomComponent;
using Combat;

namespace Game.Manager
{
    public enum HeroSynergy
    {
        Synergy1,
        Synergy2,
        Synergy3,
        Synergy4,
        Synergy5,
    }
    public enum StateSummonHero
    {
        FirstSummon,
        SummonNing,
        FreeSummon,
        EndSummon,
    }
    public class HeroManager : Singleton<HeroManager>
    {
        [field: SerializeField] public HeroUpgradeData HeroUpgradeData { get; private set; }
        private HeroUpgradeGlobalConfig HeroUpgradeGobalConfig => HeroUpgradeGlobalConfig.Instance;
        private HeroPoolGlobalConfig HeroPoolGlobalConfig => HeroPoolGlobalConfig.Instance;
        private SummonHeroGlobalConfig SummonHeroGlobalConfig => SummonHeroGlobalConfig.Instance;

        [field: SerializeField] public ReactiveValue<int> CurStageSummon { get; private set; }
        [field: SerializeField] public ReactiveValue<Family> InventorySynergy { get; private set; } = new(Family.Yokai);
        [field: SerializeField] public ReactiveValue<Family> GachaSynergy { get; private set; } = new(Family.Yokai);

        #region Unity
        private void Start()
        {
            LoadData();
            InventorySynergy = new(Family.Yokai);
            GachaSynergy = new(Family.Yokai);
        }
        #endregion

        #region Save & Load
        private void LoadData()
        {
            InitNewData();
        }
        private void SaveData()
        {
            //DatabaseManager.Instance.SaveUserDataAsync(InGameDataManager.Instance.UserData);
        }

        private void InitNewData()
        {
            for(int i = 0; i < HeroPoolGlobalConfig.HeroConfigDataArray.Length; i++)
            {
                EachHeroUpgradeData data;
                if (HeroPoolGlobalConfig.HeroConfigDataArray[i].HeroFamily != Family.Elf
                    && HeroPoolGlobalConfig.HeroConfigDataArray[i].HeroFamily != Family.Yokai
                    && HeroPoolGlobalConfig.HeroConfigDataArray[i].HeroFamily != Family.Orc)
                {
                    data = new(HeroPoolGlobalConfig.HeroConfigDataArray[i].HeroId, 0, -1);
                }
                else
                {
                   data = new(HeroPoolGlobalConfig.HeroConfigDataArray[i].HeroId, 1, 0);
                }
                HeroUpgradeData.Data.Add(data);
            }
            CurStageSummon = new(SummonHeroGlobalConfig.StartStage);
        }
        #endregion

        #region Get Set
        public HeroConfigData GetHeroConfigData(int heroId)
        {
            return HeroPoolGlobalConfig.GetHeroConfigData(heroId);
        }
        public EachHeroUpgradeData GetHeroUpgradeData(int heroId)
        {
            return HeroUpgradeData.Data.Find(x => x.HeroId == heroId);
        }
        #endregion

        #region Hero Upgrade
        public EachHeroUpgradeData GetEachHeroUpgradeData(int heroId)
        {
           return HeroUpgradeData.Data.Find(x => x.HeroId == heroId);
        }
        public int GetPieceNeededUpgradeHero(int heroId)
        {
            EachHeroUpgradeData data = HeroUpgradeData.Data.Find(x => x.HeroId == heroId);
            if(data == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroUpgradeData");
                return 0;
            }
            HeroConfigData heroConfigData = HeroPoolGlobalConfig.GetHeroConfigData(heroId);
            if(heroConfigData == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroPoolGlobalConfig");
                return 0;
            }
            HeroUpgradeLevelConfig heroLevelUpgradeConfig = HeroUpgradeGobalConfig.GetHeroUpgradeLevelConfig(heroConfigData.Rarity, data.Level);
            return heroLevelUpgradeConfig.pieceCost;
        }
        public bool IsUpgradeAbleHero(int heroId)
        {
            EachHeroUpgradeData data = HeroUpgradeData.Data.Find(x => x.HeroId == heroId);
            if(data == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroUpgradeData");
                return false;
            }
            HeroConfigData heroConfigData = HeroPoolGlobalConfig.GetHeroConfigData(heroId);
            if(heroConfigData == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroPoolGlobalConfig");
                return false;
            }
            HeroUpgradeLevelConfig heroLevelUpgradeConfig = HeroUpgradeGobalConfig.GetHeroUpgradeLevelConfig(heroConfigData.Rarity, data.Level);
            if(heroConfigData.Rarity == Combat.Rarity.Mythic)
            {
                return IsHaveEnoughResourceUpgradeHero(heroId);
            }
            return data.Piece >= heroLevelUpgradeConfig.pieceCost /*&& IsHaveEnoughResource*/;
        }
        public bool IsHaveEnoughResourceUpgradeHero(int heroId)
        {
            EachHeroUpgradeData data = HeroUpgradeData.Data.Find(x => x.HeroId == heroId);
            if (data == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroUpgradeData");
                return false;
            }
            HeroConfigData heroConfigData = HeroPoolGlobalConfig.GetHeroConfigData(heroId);
            if (heroConfigData == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroPoolGlobalConfig");
                return false;
            }
            HeroUpgradeLevelConfig heroLevelUpgradeConfig = HeroUpgradeGobalConfig.GetHeroUpgradeLevelConfig(heroConfigData.Rarity, data.Level);
            if (heroConfigData.Rarity == Combat.Rarity.Mythic)
            {
                //TODO: Check mysthic resource
                //return InGameDataManager.Instance.UserData.UserResourceList.;
                return true;
            }
            else
            {
                return true;
                //TODO: Check gold
            }
        }
        public void  UpgradeHero(int heroId)
        {
            if(!IsUpgradeAbleHero(heroId)) return;
            EachHeroUpgradeData data = HeroUpgradeData.Data.Find(x => x.HeroId == heroId);
            if(data == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroUpgradeData");
                return;
            }
            HeroConfigData heroConfigData = HeroPoolGlobalConfig.GetHeroConfigData(heroId);
            if(heroConfigData == null)
            {
                Debug.LogError($"HeroId {heroId} is not found in HeroPoolGlobalConfig");
                return;
            }
            HeroUpgradeLevelConfig heroLevelUpgradeConfig = HeroUpgradeGobalConfig.GetHeroUpgradeLevelConfig(heroConfigData.Rarity, data.Level);
            if (heroConfigData.Rarity == Combat.Rarity.Mythic)
            { 
                //TODO: Consume mysthic resource
            }
            else
            {
                data.AddLevel(1);
                data.RemovePiece(heroLevelUpgradeConfig.pieceCost);
                //TODO : Consume gold
            }
        }
        public void AddHeroPiece(int heroId, int piece)
        {
            HeroConfigData heroConfigData = HeroPoolGlobalConfig.GetHeroConfigData(heroId);
            switch (heroConfigData.Rarity)
            {
                case Combat.Rarity.Common:
                case Combat.Rarity.Uncommon:
                case Combat.Rarity.Rare:
                case Combat.Rarity.Epic:
                case Combat.Rarity.Legendary:
                    AddHeroPieceNotMythic(heroId, piece);
                    break;
                case Combat.Rarity.Mythic:
                    AddHeroPieceMythic(heroId, piece);
                    break;
                default:
                    Debug.LogError($"Rarity {heroConfigData.Rarity} is not found in AddHeroPiece");
                    break;
            }

        }
        private void AddHeroPieceMythic(int heroId, int piece)
        {
            EachHeroUpgradeData heroUpgradeData = GetEachHeroUpgradeData(heroId);
            if (heroUpgradeData.Level < 1)
            {
                heroUpgradeData.AddLevel(1);
                piece -= 1;
            }
            if (piece > 0)
            {
                //TODO : Add mythic resource 
            }
        }
        private void AddHeroPieceNotMythic(int heroId, int piece)
        {
            EachHeroUpgradeData heroUpgradeData = GetEachHeroUpgradeData(heroId);
            if(heroUpgradeData.Level < 1)
            {
                heroUpgradeData.AddLevel(1);
                piece -= 1;
            }
            if (piece > 0)
            {
                heroUpgradeData.AddPiece(piece);
            }
        }
        public float GetHeroPieceProcess(int heroId)
        {
            EachHeroUpgradeData heroUpgradeData = GetEachHeroUpgradeData(heroId);
            HeroConfigData heroConfigData = GetHeroConfigData(heroId);
            HeroUpgradeLevelConfig heroLevelUpgradeConfig = HeroUpgradeGobalConfig.GetHeroUpgradeLevelConfig(heroConfigData.Rarity, heroUpgradeData.Level);
            if(heroConfigData.Rarity == Combat.Rarity.Mythic)
            {
                //TODO : Get mythic resource
            }
            return (float)heroUpgradeData.Piece / heroLevelUpgradeConfig.pieceCost;
        }
        #endregion

        #region Hero Summon
        public bool IsGachaAbleFamily()
        {
            return GachaSynergy.Value == Family.Yokai || GachaSynergy.Value == Family.Orc || GachaSynergy.Value == Family.Elf;
        }
        public void ChangeGachaSynergy(Family family)
        {
            GachaSynergy.Value = family;
        }
        public void SetCurrentStageSummon(int stage)
        {
            CurStageSummon.Value = stage;
        }
        public int GetNumAppearance()
        {
            return SummonHeroGlobalConfig.NumAppearanceProbability.GetRandomItem();
        }
        public List<SummonHeroConfig> GetListSummonHeroConfig(int numAppearance)
        {
            List<SummonHeroConfig> list = new List<SummonHeroConfig>();
            for (int i = 0; i < numAppearance; i++)
            {
                list.Add(SummonHeroGlobalConfig.SummonHeroConfigProbability.GetRandomItem());
            }
            return list;
        }
        public List<SummonHeroReward> GetListSummonHeroReward(List<SummonHeroConfig> input, int timeSummon)
        {
            List<SummonHeroReward> list = new List<SummonHeroReward>();
            foreach (SummonHeroConfig summonHeroConfig in input)
            {
                if (summonHeroConfig.IsAcquired())
                {
                    list.Add(new SummonHeroReward(summonHeroConfig,true, timeSummon, GachaSynergy));
                }
                else
                {
                    list.Add(new SummonHeroReward(summonHeroConfig, false,timeSummon, GachaSynergy));
                }
            }
            return list;
        }
        public HeroConfigData GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity rarity, Family family)
        {
            return HeroPoolGlobalConfig.GetRandomHeroConfigDataByRarityAndFamily(rarity, family);
        }
        public void ClaimSummonReward(List<SummonHeroReward> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!list[i].IsAcquired) continue;
                list[i].ClaimReward();
            }
        }
        public void UpgradeStageSummon(int value)
        {
            CurStageSummon.Value = value;
        }
        //private void ClaimSummonRewardHeroPiece(SummonHeroReward summonHeroReward)
        //{
        //    HeroConfigData heroConfigData = new();
        //    //switch(summonHeroReward.Config.Type)
        //    //{
        //    //    case SummonHeroType.HeroCommon:
        //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Common);
        //    //        break;
        //    //    case SummonHeroType.HeroUncommon:
        //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Common);
        //    //        break;
        //    //    case SummonHeroType.HeroRare:
        //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Rare);
        //    //        break;
        //    //    case SummonHeroType.HeroEpic:
        //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Epic);
        //    //        break;
        //    //    case SummonHeroType.HeroMythic:
        //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Mythic);
        //    //        break;
        //    //    case SummonHeroType.HeroLegendary:
        //    //        heroConfigData = GetRandomHeroConfigDataByRarityAndFamily(Combat.Rarity.Mythic);
        //    //        break;
        //    //    default:
        //    //        Debug.LogError($"SummonHeroType {summonHeroReward.Config.Type} is not found in ClaimSummonRewardHeroPiece");
        //    //        break;
        //    //}
        //    EachHeroUpgradeData heroUpgradeData = GetEachHeroUpgradeData(heroConfigData.HeroId);
        //    AddHeroPiece(heroUpgradeData.HeroId, summonHeroReward.Config.RewardAmount);
        //}
        //private void ClaimSummonRewardResource(SummonHeroReward summonHeroReward)
        //{
        //    //TODO : Add resource
        //}

        #endregion

        #region Hero Synergy
        public List<HeroConfigData> GetListHeroConfigDataBySynergy(Family synergy)
        {
            return HeroPoolGlobalConfig.GetListHeroConfigDataBySynergy(synergy);
        }
        public void ChangeInventorySynergy(Family synergy)
        {
            InventorySynergy.Value = synergy;
        }
        public bool IsUnlockSynergy(Family synergy)
        {
            return synergy == Family.Yokai || synergy == Family.Orc || synergy == Family.Elf;
        }
        #endregion
    }
}

