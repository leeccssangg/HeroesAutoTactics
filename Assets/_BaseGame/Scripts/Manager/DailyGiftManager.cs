using System;
using UnityEngine;
using System.Collections.Generic;
using TW.Utility.DesignPattern;
using MemoryPack;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
using Game.Manager;
using Game.Data;
using Cysharp.Threading.Tasks;

public class DailyGiftManager : Singleton<DailyGiftManager>
{
    [SerializeField] public DailyGiftData m_DailyGiftData;
    [SerializeField] private List<int> m_ListIDCollected;
    [SerializeField] private DateTime m_ResetDay;
    [SerializeField] public ReactiveValue<int> NumCollected { get; private set; } = new(0);
    [SerializeField] public ReactiveValue<TimeSpan> TimeToReset { get; private set; } = new(TimeSpan.Zero);
    [SerializeField] public ReactiveValue<int> Stage { get; private set; } = new(0);

    private void Start()
    {
        LoadData();
    }
    private void Update()
    {
        DateTime currentTime = DateTime.Now;
        if (DateTime.Compare(currentTime, m_ResetDay) >= 0)
        {
            LoadData();
            //UpdateNextDay();
        }
        TimeToReset.Value = m_ResetDay.Subtract(currentTime);
    }
    public void LoadData()
    {
        //m_DailyGiftData = InGameDataManager.Instance.InGameData.DailyGiftData;
        m_DailyGiftData = new();
        ImportDataString(m_DailyGiftData.ListIDCollected, m_DailyGiftData.ResetDay, m_DailyGiftData.Stage);
    }
    public void ResetDataNewDay()
    {
        LoadData();
        Save();
    }
    public void Save()
    {
        m_DailyGiftData.ListIDCollected = m_ListIDCollected;
        m_DailyGiftData.ResetDay = m_ResetDay;
        m_DailyGiftData.Stage = Stage.Value;
        //InGameDataManager.Instance.SaveData();
    }
    public void ImportDataString(List<int> id, DateTime resetDay, int stage)
    {
        m_ResetDay = resetDay;
        if(IsResetNewReward())
        {
            m_ListIDCollected =  new();
            m_ResetDay = DateTime.Now.AddDays(30);
            Stage.Value = 0;
        }
        else
        {
            m_ListIDCollected = id;
            Stage.Value = stage;
        }
    }
    private int GetRealTimeDayId()
    {
        TimeSpan ts = m_ResetDay - DateTime.Now;
        return 29 - (int)ts.TotalDays;
    }
    private bool IsResetNewReward()
    {
       TimeSpan ts = DateTime.Now - m_ResetDay;
        return ts.TotalSeconds >= 0;
    }
    public bool IsGoodToClaim(int dayID)
    {
        return dayID == GetRealTimeDayId() && !m_ListIDCollected.Contains(GetRealTimeDayId());
    }
    public UserResource GetDailyGift()
    {
        UserResource gift = DailyGiftGlobalConfig.Instance.GetDailyGiftByDayNum(GetRealTimeDayId());
        return gift;
    }
    public async UniTask OnClaimComplete(int dayID)
    {
        UserResource gift = DailyGiftGlobalConfig.Instance.GetDailyGiftByDayNum(dayID);
        var response = await DatabaseManager.Instance.PostUserResourceChangeData(gift.ResourceType, gift.Amount);
        if (response.IsFaulted)
        {
            UnityEngine.Debug.Log("Failed to change resource");
        }
        else if (response.IsSucceeded)
        {
            m_ListIDCollected.Add(dayID);
            NumCollected.Value = m_ListIDCollected.Count;
            NotiDailyGift();
            Save();
        }
    }
    public async UniTask OnClaimStageReward()
    {
        UserResource gift = GetListDailyGiftStages()[Stage].reward;
        var response = await DatabaseManager.Instance.PostUserResourceChangeData(gift.ResourceType, gift.Amount);
        if (response.IsFaulted)
        {
            UnityEngine.Debug.Log("Failed to change resource");
        }
        else if (response.IsSucceeded)
        {
            Stage.Value++;
            Save();
        }

    }
    public int GetCurrentDayId()
    {
        return GetRealTimeDayId();
    }
    public List<DailyReward> GetListDailyGifts()
    {
        return DailyGiftGlobalConfig.Instance.GetListDailyGifts();
    }
    public List<DailyGiftStage> GetListDailyGiftStages()
    {
        return DailyGiftGlobalConfig.Instance.stages;
    }
    public DailyGiftStage GetDailyGiftStageByDayCount(int dayCount)
    {
        return DailyGiftGlobalConfig.Instance.GetDailyGiftStageByDayCount(dayCount);
    }
    public string GetTimeToResetReward()
    {
        TimeSpan timeSpan = m_ResetDay - DateTime.Now;
        string time = TimeUtil.TimeToString(timeSpan.TotalSeconds);
        return time;
    }
    public void NotiDailyGift()
    {
        //SheetMainMenu.Events.NotiDailyGift?.Invoke(IsGoodToClaim());
    }
    public bool IsMissingDay(int day)
    {
        return day < GetRealTimeDayId() && !m_ListIDCollected.Contains(day);
    }
    public bool IsCollectedDayID(int day)
    {
        return m_ListIDCollected.Contains(day) && day <= GetRealTimeDayId();
    }
    public bool IsNotReadyCollectDayId(int day)
    {
       return day > GetRealTimeDayId();
    }
    public float GetProcess()
    {
        float tmp = (float)NumCollected / 28;
        if (tmp > 1) tmp = 1;
        return tmp;
    }
    public bool IsGoodToClaimStageReward()
    {
        if(Stage.Value == DailyGiftGlobalConfig.Instance.stages[^1].dayCount)
        {
            return false;
        }
        if(NumCollected.Value >= DailyGiftGlobalConfig.Instance.stages[Stage.Value].dayCount)
        {
            return true;
        }
        return false;
    }
}
[System.Serializable]
public class DailyReward
{
    public int dayID;
    public UserResource defaultGift;

    public DailyReward(int id, UserResource defaultG)
    {
        dayID = id;
        defaultGift = defaultG;
    }
}
[System.Serializable]
public class DailyGiftStage
{
    public int dayCount;
    public UserResource reward;

    public DailyGiftStage(int id, UserResource defaultG)
    {
        dayCount = id;
        reward = defaultG;
    }
}
[System.Serializable]
public partial class DailyGiftData
{
    [field: SerializeField] public List<int> ListIDCollected { get; set; } = new();
    [field: SerializeField] public DateTime ResetDay { get; set; } = DateTime.Now.AddDays(30);
    [field: SerializeField] public int Stage { get; set; } = 0;

    public DailyGiftData()
    {
        //Id = new(0);
        //Date = DateTime.Now;
        ListIDCollected = new List<int>();
        ResetDay = DateTime.Now.AddDays(30);
        Stage = 0;
    }
}
//public partial class InGameData
//{
//    [MemoryPackOrder(4)]
//    [field: SerializeField, PropertyOrder(4)] public DailyGiftData DailyGiftData { get; set; } = new();
//}
