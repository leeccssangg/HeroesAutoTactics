using System.Collections.Generic;
using UnityEngine;
using Sirenix.Utilities;

[CreateAssetMenu(fileName = "PrefabGlobalConfig", menuName = "GlobalConfigs/PrefabGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class PrefabGlobalConfig : GlobalConfig<PrefabGlobalConfig>
{


    

}
[System.Serializable]
public class PrefabConfig<T>
{
    [field: SerializeField] public int Id { get; private set; }
    [field: SerializeField] public T Prefab { get; private set; }
}