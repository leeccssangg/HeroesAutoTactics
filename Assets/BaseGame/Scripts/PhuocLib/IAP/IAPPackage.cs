using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum IAP_TYPE {
    NORMAL = 0,
    REMOVEADS,
}
[System.Serializable]
public class IAPPackage {
    public int packID;
    public string productID;
    public string primeProductID;
    public string price;
    public string currencyCode = "USD";
    public string localizedPriceString;
    public decimal localizedPrice;
    public int salePercent;
    public int saleValue = 1;
    public int nameID;
    public int vipPoint;
    public int descriptionID = -1;
    public int cooldownDay = 3;
    public int subsTime = 0;
    public int totalSubEarn = 0;
    public bool isConnectedToStore = false;
    public bool isDouble = false;
    public IAP_TYPE iapType;
    // public List<ItemPackage> m_ItemPackages = new List<ItemPackage>();
    public IAPPackage(int _packID,string _productID,IAP_TYPE _iapType,int _nameID,int _desID = -1) {
        packID = _packID;
        productID = _productID;
        nameID = _nameID;
        descriptionID = _desID;
        iapType = _iapType;
    }
    public IAPPackage(string _productID) {
        productID = _productID;
    }
    public string GetID() {  
        return productID;
    }
    public int GetCooldownTime() {
        return cooldownDay;
    }
    public void SetCooldownTime(int day) {
        cooldownDay = day;
    }
    public void SetSalePercent(int _per) {
        salePercent = _per;
    }
    public int GetSalePercent() {
        return salePercent;
    }
    public void SetPrice(string _Price) {
        price = _Price;
        localizedPriceString = _Price;
    }
    public string GetPrice() {
        string s = price;
        //Debug.Log("pid " + productID + " connect " + isConnectedToStore + " Price " + localizedPriceString);
        if(isConnectedToStore) {
            s = localizedPriceString;
        } else {
            s = price;
        }
        return s;
    }
    // public int GetDefaultEarn() {
    //     return m_ItemPackages[0].GetAmount();
    // }
    //
    // public int GetGemEarn() {
    //     int num = m_ItemPackages[0].GetAmount();
    //     if(isDouble)
    //         num *= 2;
    //     //if (isEffectByK) {
    //     //    double k = GameManager.Instance.GetKIAPConfig();
    //     //    num = (int)(num * k);
    //     //}
    //     return num;
    // }
    public int GetTotalEarn() {
        int num = totalSubEarn;
        //if (isEffectByK) {
        //    double k = GameManager.Instance.GetKIAPConfig();
        //    num = (int)(num * k);
        //}
        return num;
    }
    public int GetVipPoint() {
        return vipPoint;
    }
    // public int GetCurrentGemEarn() {
    //     int ratio = isDouble ? 2 : 1;
    //     return m_ItemPackages[0].GetAmount() * ratio;
    // }
    // public void AddItemPackages(ItemPackage ip) {
    //     m_ItemPackages.Add(ip);
    // }
    // public List<ItemPackage> GetItemPacks() {
    //     return new List<ItemPackage>(m_ItemPackages);
    // }
    //public string GetTitle() {
    //    return GameData.Instance.GetGameTextByID(nameID);
    //}
    //public string GetDescription() {
    //    return GameData.Instance.GetGameTextByID(descriptionID);
    //}
    // public bool IsDoublePack() {
    //     return iapType == IAP_TYPE.DOUBLE_GEM;
    // }
    // public bool IsSubscription() {
    //     return iapType == IAP_TYPE.SUBCRIPTION;
    // }
    // public ItemPackage GetDailySubRewards() {
    //     return m_ItemPackages[1];
    // }
    // public ItemPackage GetOnetimeSubGift() {
    //     return m_ItemPackages[0];
    // }
    // public List<ItemPackage> GetRewards() {
    //     return m_ItemPackages;
    // }
}
