using System;
using System.Collections.Generic;
using UnityEngine;
using TW.Utility.DesignPattern;
using MemoryPack;
using Sirenix.OdinInspector;
using TW.Reactive.CustomComponent;
//using SDK;

public class InAppPurchaseManager : Singleton<InAppPurchaseManager>
{
    //[field: SerializeField] public List<IAPPackage> ConsumablePackages { get; private set; } = new List<IAPPackage>();
    [field: SerializeField] public ShopData ShopD { get; private set; } = new ShopData();

    [field: SerializeField] public List<IAPPackage> IAPPackages { get; private set; } = new List<IAPPackage>();

    private ConsumableIAPGlobalConfig ConsumableIAPConfig => ConsumableIAPGlobalConfig.Instance;
    private UnconsumableIAPGlobalConfig UnconsumableIAPConfig => UnconsumableIAPGlobalConfig.Instance;

    [field: SerializeField] public ReactiveValue<bool> IsRemoveAds { get; private set; }

    public List<IAPPackage> IAPPackageList
    {
        get => IAPPackages;
        set => IAPPackages = value;
    }

    private void Start()
    {
        //m_RemoveAdsConfig = RemoveAdsConfig.Instance;
        LoadData();
        LoadIAPPackages();
    }
    private void LoadData()
    {
        //ShopD = InGameDataManager.Instance.InGameData.ShopData;
        //IsRemoveAds = ShopD.IsRemoveAds;
        //if(IsNextDay())
        //{
        //    ShopD.FreeAdsMoneyD.NextTime = DateTime.Now;
        //    ShopD.FreeAdsOpenChestD.NextTime = DateTime.Now;
        //}
        //UpdateNextDay();
        //TimeSpan t = DateTime.Now - ShopD.ExpireRemoveAds.Value.Date;
        //if (t.TotalSeconds >= 0)
        //{
        //    AdsManager.Instance.SetRemoveAdsExpired(false);
        //}
        //SaveData();
        //Debug.Log(TimeUtil.DateTimeToString(ShopD.FreeAdsMoneyD.NextTime));
    }
    private bool IsNextDay()
    {
        TimeSpan t = DateTime.Now - ShopD.NextDay.Date;
        return t.TotalSeconds >= 0;
    }
    public bool IsExpireRemoveAds()
    {
        TimeSpan t = DateTime.Now - ShopD.ExpireRemoveAds.Value.Date;
        return t.TotalSeconds >= 0;
    }
    private void UpdateNextDay()
    {
        ShopD.NextDay = TimeUtil.GetNextDate();
    }
    private void LoadIAPPackages()
    {
        IAPPackages = new List<IAPPackage>();
        LoadConsumablePackages();
        LoadUnConsumablePackages();
    }
    private void LoadConsumablePackages()
    {
        for(int i = 0;i< ConsumableIAPConfig.removeAdsConfigs.Count; i++)
        {
            IAPPackage removeAds = new IAPPackage(ConsumableIAPConfig.removeAdsConfigs[i].productId);
            removeAds.SetPrice("$" + ConsumableIAPConfig.removeAdsConfigs[i].price.ToString());
            IAPPackages.Add(removeAds);
        }
    }
    private void LoadUnConsumablePackages()
    {
        for(int i = 0;i < UnconsumableIAPConfig.packages.Count; i++)
        {
            IAPPackage package = new IAPPackage(UnconsumableIAPConfig.packages[i].productId);
            package.SetPrice("$" + UnconsumableIAPConfig.packages[i].price.ToString());
            IAPPackages.Add(package);
        }
    }
    public IAPPackage GetIAPPackage(string productId)
    {
        return IAPPackageList.Find(x => x.productID.Equals(productId));
    }
    public void OnByRemoveAds()
    {
// #if UNITY_EDITOR
//         OnBuyIAPSuccess(Purchaser.k_removeads);
//         OnBuySuccess();
//         return;
// #endif
        Purchaser.Instance.BuyIAPProduct(Purchaser.super_no_ads, OnBuySuccess, null);
        return;
        void OnBuySuccess()
        {
            // Add reward
            //List<GameResource> list = ConsumableIAPConfig.GetIapPackageById("1").gameResource;
            //foreach (GameResource t in list)
            //{
            //    PlayerResource.Add(t);
            //    //ProfileManager.PlayerData.AddGameResource(t.type, t.value,"");
            //}
            //AdsManager.Instance.HideBannerAds();
            // UIManager.Ins.CloseUI<UIPanelBanner>();
            //UIManager.Ins.GetUI<UIPanelRemoveAds>().TweenClose();
        }
    }
    public void OnBuyIAPSuccess(string productId)
    {
        switch (productId)
        {
            case Purchaser.super_no_ads:
            case Purchaser.starter_pack:
                IsRemoveAds.Value = true;
                //AdsManager.Instance.SetRemoveAds(true);
                break;
            case Purchaser.no_ads:
                OnBuyAdsExpire(365);
                break;

        }
        Debug.Log(productId);
        //SaveData();
    }
    private void OnBuyAdsExpire(int dayCount)
    {
        Debug.Log(TimeUtil.DateTimeToString(ShopD.ExpireRemoveAds.Value));
        if (IsExpireRemoveAds())
        {
            ShopD.ExpireRemoveAds.Value = DateTime.Now.AddDays(dayCount);
        }
        else
        {
            ShopD.ExpireRemoveAds.Value= ShopD.ExpireRemoveAds.Value.AddDays(dayCount);
        }
        Debug.Log(TimeUtil.DateTimeToString(ShopD.ExpireRemoveAds.Value));
        //AdsManager.Instance.SetRemoveAdsExpired(true);
        //SaveData();
    }
    //public bool IsEnoughGemToBuyMoneyPack(MoneyPack pack)
    //{
    //    return PlayerResource.Get(pack.price.ResourceType).Amount >= pack.price.Amount;
    //}
    //public bool IsEnoughGemToBuyChestPack(ChestPack pack)
    //{
    //    return PlayerResource.Get(pack.price.ResourceType).Amount >= pack.price.Amount;
    //}
    //public void OnBuyMoneyPack(MoneyPack moneyPack)
    //{
    //    if (moneyPack.price.Amount > 0)
    //    {
    //        PlayerResource.Consume(moneyPack.price);
    //        OnBuyMoneyPackCompleted(moneyPack);
    //    }
    //    else
    //    {
    //        AdsController.ShowRewardedVideo("ShopMoney",() =>
    //        {
    //            OnBuyMoneyPackCompleted(moneyPack);
    //            ShopD.FreeAdsMoneyD.NextTime = DateTime.Now.AddMinutes(60 * 2);
    //            AllQuestManager.Instance.Notify(MissionTarget.GET_FREE_BUCKS, "1");
    //        },null,null,null);
    //        //FreeMoneyNextTime = DateTime.Now.AddSeconds(10);
    //    }
    //    SaveData();
    //}
    //private void OnBuyMoneyPackCompleted(MoneyPack moneyPack)
    //{
    //    PlayerResource.Add(moneyPack.reward);
    //}
    //public void OnBuyChestPack(ChestPack chesPack,bool isAds)
    //{
    //    if(!isAds)
    //    {
    //        PlayerResource.Consume(chesPack.price);
    //        OnBuyChestPackCompleted(chesPack);
    //    }
    //    else
    //    {

    //        AdsController.ShowRewardedVideo("ShopChest", () =>
    //        {
    //            OnBuyChestPackCompleted(chesPack);
    //            ShopD.FreeAdsOpenChestD.NextTime = DateTime.Now.AddMinutes(60 * 2);
    //            AllQuestManager.Instance.Notify(MissionTarget.OPEN_FREE_CHEST, "1");
    //        }, null, null, null);
    //    }
      
    //    SaveData();
    //}
    //private void OnBuyChestPackCompleted(ChestPack chestPack)
    //{
    //    PlayerResource.Add(chestPack.reward);
    //    if(chestPack.reward.ResourceType != GameResource.Type.CommonChest) return;
    //    ShopD.FreeAdsOpenChestD.OpenedCount.Value++;
    //    if (ShopD.FreeAdsOpenChestD.OpenedCount.Value % 10 == 0)
    //    {
    //        PlayerResource.Add(GameResource.Type.CommonChest, 1);
    //    }
    //}
    //private void SaveData()
    //{
    //    InGameDataManager.Instance.SaveData();
    //    //Debug.Log(TimeUtil.DateTimeToString(ShopD.FreeAdsMoneyD.NextTime));
    //    //Debug.Log(TimeUtil.DateTimeToString(InGameDataManager.Instance.InGameData.ShopData.FreeAdsMoneyD.NextTime));
    //}
    public bool IsFreeOpenChest()
    {
        TimeSpan t = DateTime.Now - ShopD.FreeAdsOpenChestD.NextTime;
        return t.TotalSeconds >= 0;
    }
    public bool IsFreeMoney()
    {
        TimeSpan t = DateTime.Now - ShopD.FreeAdsMoneyD.NextTime;
        return t.TotalSeconds >= 0;
    }
    public string GetTimeToNextFreeOpenChest()
    {
        TimeSpan t = ShopD.FreeAdsOpenChestD.NextTime - DateTime.Now;
        return TimeUtil.TimeToString(t.TotalSeconds);
    }
    public string GetTimeToNextFreeMoney()
    {
        TimeSpan t = ShopD.FreeAdsMoneyD.NextTime - DateTime.Now;
        return TimeUtil.TimeToString(t.TotalSeconds);
    }
    public string GetTimeToExpireRemoveAds()
    {
        TimeSpan t = ShopD.ExpireRemoveAds.Value - DateTime.Now;
        return TimeUtil.TimeToString(t.TotalSeconds);
    }
}
[Serializable]
[MemoryPackable]
public partial class ShopData
{
    [field: SerializeField] public ReactiveValue<bool> IsRemoveAds { get; set; } = new(false);
     public DateTime NextDay { get; set; } = DateTime.Now;
     public ReactiveValue<DateTime> ExpireRemoveAds { get; set; } = new(DateTime.Now);
    [field: SerializeField] public FreeAdsOpenChestData FreeAdsOpenChestD { get; set; } = new();
    [field: SerializeField] public FreeAdsMoneyData FreeAdsMoneyD { get; set; } = new();

    [MemoryPackConstructor]
    public ShopData()
    {
        IsRemoveAds = new(false);
        NextDay = DateTime.Now;
        ExpireRemoveAds = new(DateTime.Now);
        FreeAdsOpenChestD = new();
        FreeAdsMoneyD = new();
    }
}
[Serializable]
[MemoryPackable]
public partial class FreeAdsOpenChestData
{
    public DateTime NextTime { get; set; } = DateTime.Now;
    [field: SerializeField] public ReactiveValue<int> OpenedCount { get; set; } = new(0);

    [MemoryPackConstructor]
    public FreeAdsOpenChestData()
    {
        NextTime = DateTime.Now;
        OpenedCount = new(0);
    }
}
[Serializable]
[MemoryPackable]
public partial class FreeAdsMoneyData
{
    public DateTime NextTime { get; set; } = DateTime.Now;

    [MemoryPackConstructor]
    public FreeAdsMoneyData()
    {
        NextTime = DateTime.Now;
    }
}
public partial class InGameData
{
    [MemoryPackOrder(8)]
    [field: SerializeField, PropertyOrder(8)] public ShopData ShopData { get; set; } = new();
}

