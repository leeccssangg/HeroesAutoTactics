using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "ConsumableIAPGlobalConfig", menuName = "GlobalConfigs/ConsumableIAPGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ConsumableIAPGlobalConfig : GlobalConfig<ConsumableIAPGlobalConfig>
{
    public List<RemoveAdsConfig> removeAdsConfigs = new List<RemoveAdsConfig>();

    public RemoveAdsConfig GetIapPackageById(string id)
    {
        for(int i = 0;i< removeAdsConfigs.Count; i++)
        {
            if (removeAdsConfigs[i].productId.Equals(id))
            {
                return removeAdsConfigs[i];
            }
        }
        return null;
    }
}
[System.Serializable]
public class RemoveAdsConfig
{
    public string productId;
    public float price;
    //public List<GameResource> gameResource = new List<GameResource>();
}
