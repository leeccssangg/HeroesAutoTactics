using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using Game.Data;
using Sirenix.OdinInspector;
using TW.Utility.Extension;
using UnityEditor;
using Unity.VisualScripting;

[CreateAssetMenu(fileName = "DailyGiftGlobalConfig", menuName = "GlobalConfigs/DailyGiftGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class DailyGiftGlobalConfig : GlobalConfig<DailyGiftGlobalConfig>
{
    public List<DailyReward> gifts = new();
    public List<DailyGiftStage> stages = new();

    public List<DailyReward> GetListDailyGifts()
    {
        return new List<DailyReward>(gifts);
    }
    public UserResource GetDailyGiftByDayNum(int daynum)
    {
        UserResource item = new(gifts[daynum].defaultGift.ResourceType, gifts[daynum].defaultGift.Amount);
        return item;
    }
    public UserResource GetDailyGiftStageRewardByDayCount(int dayCount)
    {

       UserResource item = new(stages[dayCount].reward.ResourceType, stages[dayCount].reward.Amount);
        return item;
    }
    public DailyGiftStage GetDailyGiftStageByDayCount(int dayCount)
    {
        for (int i = 0; i < stages.Count; i++)
        {
            if (stages[i].dayCount == dayCount)
            {
                return stages[i];
            }
        }
        return null;
    }
#if UNITY_EDITOR
    [BoxGroup("Sheet Info")] public GoogleSheetDefinder m_SheetDailyGiftConfig;

    private List<Dictionary<string, string>> m_DailyGiftData;
    [Button]
    public async void LoadDailyGift()
    {
        string csvString = await ABakingSheet.GetCsv(m_SheetDailyGiftConfig.linkSheetID, m_SheetDailyGiftConfig.linkSheetTabName);
        EditorUtility.SetDirty(this);
        if (m_DailyGiftData.IsNullOrEmpty()) m_DailyGiftData = CSVReader.ReadDataFromString(csvString);
        gifts.Clear();
        stages.Clear();
        for (int i = 0;i< m_DailyGiftData.Count; i++)
        {
            int id = m_DailyGiftData[i]["Day"].ConvertToInt()-1;
            UserResource.Type type = m_DailyGiftData[i]["Type1"].ConvertToEnum<UserResource.Type>();
            int amount = m_DailyGiftData[i]["Amount1"].ConvertToInt();
            if (m_DailyGiftData[i]["Type2"].IsNullOrWhitespace())
            {
                gifts.Add(new DailyReward(id, new UserResource(type, new(amount))));
            }
            else
            {
                UserResource.Type type2 = m_DailyGiftData[i]["Type2"].ConvertToEnum<UserResource.Type>();
                int amount2 = m_DailyGiftData[i]["Amount2"].ConvertToInt();
                gifts.Add(new DailyReward(id, new UserResource(type, new(amount))));
                stages.Add(new DailyGiftStage(id+1, new UserResource(type2, new(amount2))));
            }
            
        }
        
    }
#endif
}