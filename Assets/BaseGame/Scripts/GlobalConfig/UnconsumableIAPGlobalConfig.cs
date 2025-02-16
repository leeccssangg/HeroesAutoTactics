using UnityEngine;
using Sirenix.Utilities;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "UnconsumableIAPGlobalConfig", menuName = "GlobalConfigs/UnconsumableIAPGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class UnconsumableIAPGlobalConfig : GlobalConfig<UnconsumableIAPGlobalConfig>
{
    public List<IAPBundle> packages = new List<IAPBundle>();

    public IAPBundle GetIapPackageById(string productId)
    {
        return packages.Find(x => x.productId.Equals(productId));
    }
}
[Serializable]
public class IAPBundle
{
    public string productId;
    public float price;
    //public List<GameResource> gameResource = new List<GameResource>();
}
