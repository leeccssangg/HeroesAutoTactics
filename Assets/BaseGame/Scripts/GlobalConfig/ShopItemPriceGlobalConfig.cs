using UnityEngine;
using Sirenix.Utilities;
using TW.Utility.CustomType;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ShopItemPriceGlobalConfig", menuName = "GlobalConfigs/ShopItemPriceGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class ShopItemPriceGlobalConfig : GlobalConfig<ShopItemPriceGlobalConfig>
{
    public List<MoneyPack> MoneyPacks;
    public List<ChestPack> ChestPacks;
}
[System.Serializable]
public class MoneyPack
{
    //public GameResource price;
    //public GameResource reward;
}
[System.Serializable]
public class ChestPack
{
    //public GameResource price;
    //public GameResource reward;
}