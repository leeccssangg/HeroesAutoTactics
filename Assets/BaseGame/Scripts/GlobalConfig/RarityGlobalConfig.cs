using Combat;
using Sirenix.Utilities;
using UnityEngine;

[CreateAssetMenu(fileName = "RarityGlobalConfig", menuName = "GlobalConfigs/RarityGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class RarityGlobalConfig : GlobalConfig<RarityGlobalConfig>
{
    [field: SerializeField] public RarityConfig[] RarityConfigs { get; private set; }
    
    public RarityConfig GetRarityConfig(Rarity rarity)
    {
        return RarityConfigs[(int)rarity];
    }
}

[System.Serializable]
public class RarityConfig
{
    [field: SerializeField] public Rarity Rarity { get; private set; }
    [field: SerializeField] public Sprite RarityBackground { get; private set; }
    [field: SerializeField] public Sprite RarityStroke { get; private set; }
}