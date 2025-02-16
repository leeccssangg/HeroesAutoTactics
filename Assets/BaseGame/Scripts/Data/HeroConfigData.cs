using System.Collections.Generic;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Manager;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using Spine.Unity;
using TW.Utility.Extension;
using UnityEngine;

#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.AssetImporters;
#endif

[CreateAssetMenu(fileName = "HeroConfigData", menuName = "ScriptableObjects/HeroConfigData")]
public class HeroConfigData : ScriptableObject
{
    [field: ShowIf("@SpriteIcon == null && SkeletonDataAsset != null")]
    [field: HorizontalGroup(nameof(HeroConfigData), width: 150)]
    [field: HideLabel, PreviewField(150)]
    [field: SerializeField]
    public SkeletonDataAsset SkeletonDataAsset { get; set; }
    [field: HorizontalGroup(nameof(HeroConfigData), width: 150)]
    [field: ShowIf("@SpriteIcon != null && SkeletonDataAsset == null")]
    [field: HideLabel, PreviewField(150)]
    [field: SerializeField]
    public Sprite SpriteIcon { get; set; }
    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public Rarity Rarity { get; set; }
    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public int HeroId { get; set; }
    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public Family HeroFamily { get; set; }
    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public string HeroName { get; set; }
    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public bool IsToken { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public int AttackDamage { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public int HealthPoint { get; set; }

    [field: VerticalGroup(nameof(HeroConfigData) + "/HeroInformation")]
    [field: SerializeField]
    public Hero HeroPrefab { get; set; }
    [field: SerializeField] public AbilityPower[] AbilityPower {get; private set;}

    public AbilityPower GetAbilityPower(int heroStar)
    {
        return AbilityPower[heroStar - 1];
    }
    public string GetDescription(int heroStar)
    {
        return AbilityPower[heroStar - 1].Description;
    }

    public void Initialize(InGameHandleManager inGameHandleManager, FactoryManager factoryManager)
    {
        foreach (AbilityPower abilityPower in AbilityPower)
        {
            abilityPower.Initialize(inGameHandleManager, factoryManager);
        }
    }
    
#if UNITY_EDITOR
    [Button]
    // ReSharper disable once CognitiveComplexity
    public async UniTask GenerateAbilityPower()
    {
        int tier = (int)Rarity;
        string sheetId = "148lC2c-LMI2lP2JYH3OTOSmhi2L7KMDrQF70dlUvxYA";
        string sheetName = $"Tier{tier}";
        string scvData = await ABakingSheet.GetCsv(sheetId, sheetName);
        List<Dictionary<string, string>> database = ACsvReader.ReadDataFromString(scvData);
        EditorUtility.SetDirty(this);
        for (int i = 0; i < database.Count; i++)
        {
            if (!int.TryParse(database[i]["ID"], out int id) || id != HeroId) continue;
            
            HeroName = database[i]["HeroName"];
           
            if (int.TryParse(database[i]["AttackDamage"], out int attackDamage))
            {
                AttackDamage = attackDamage;
            }
            if (int.TryParse(database[i]["HealthPoint"], out int healthPoint))
            {
                HealthPoint = healthPoint;
            }

            if (Enum.TryParse(database[i]["HeroFamily"], out Family heroFamily))
            {
                HeroFamily = heroFamily;
            }
            try
            {
                string skeletonName = database[i]["SkeletonName"];
                if (!skeletonName.IsNullOrWhitespace())
                {
                    Debug.Log($"t:SkeletonDataAsset {skeletonName}");
                    string guid = AssetDatabase.FindAssets($"t:SkeletonDataAsset {skeletonName}",
                        new string[] { $"Assets/BaseGame/Animations/Skeletons/" })[0];
                    string skeletonPath = AssetDatabase.GUIDToAssetPath(guid);
                    SkeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(skeletonPath);
                }
                else
                {
                    SkeletonDataAsset = null;
                }
            }
            catch (Exception e)
            {
                SkeletonDataAsset = null;
                Debug.Log(e);
            }
            
            try
            {
                string guid = AssetDatabase.FindAssets($"t:Sprite {HeroName}",
                    new string[] { $"Assets/BaseGame/Graphics/Sprites/Hero/Tier{tier}/" })[0];
                string spritePath = AssetDatabase.GUIDToAssetPath(guid);
                SpriteIcon = AssetDatabase.LoadAssetAtPath<Sprite>(spritePath);
            }
            catch (Exception e)
            {
                SpriteIcon = null;
                Debug.Log(e);
            }
            HeroPrefab = null;
            Hero heroPrefab = null;
            try
            {
                string guid = AssetDatabase.FindAssets($"t:Prefab {id}_{HeroName}", new string[] { "Assets/BaseGame/Prefabs/Hero" })[0];
                heroPrefab = AssetDatabase.LoadAssetAtPath<Hero>(AssetDatabase.GUIDToAssetPath(guid));
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            if (heroPrefab == null)
            {
                Hero heroBasePrefab = AssetDatabase.LoadAssetAtPath<Hero>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("t:Prefab HeroBase")[0])); 
                heroPrefab = Instantiate(heroBasePrefab);
                heroPrefab.name = $"{id}_{HeroName}";
                PrefabUtility.SaveAsPrefabAsset(heroPrefab.gameObject,
                    $"Assets/BaseGame/Prefabs/Hero/Tier{tier}/{heroPrefab.name}.prefab");
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                
                DestroyImmediate(heroPrefab.gameObject);
                string guid = AssetDatabase.FindAssets($"t:Prefab {id}_{HeroName}", new string[] { "Assets/BaseGame/Prefabs/Hero" })[0];
                heroPrefab = AssetDatabase.LoadAssetAtPath<Hero>(AssetDatabase.GUIDToAssetPath(guid));
            }
            HeroPrefab = heroPrefab;
            using PrefabUtility.EditPrefabContentsScope scope = new PrefabUtility.EditPrefabContentsScope($"Assets/BaseGame/Prefabs/Hero/Tier{tier}/{heroPrefab.name}.prefab");
            scope.prefabContentsRoot.GetComponent<Hero>().SetHeroConfigData(this);
            
            
            string path = AssetDatabase.GetAssetPath(this);
            
            string fileName = $"{id}_{HeroName}";
            
            EditorUtility.SetDirty(this);
            
            AbilityPower = new AbilityPower[3];
            for (int j = 0; j < 3; j++)
            {
                string power = database[i+j]["AbilityPower"];
                AbilityPower[j] = AssetDatabase.LoadAssetAtPath<AbilityPower>($"Assets/BaseGame/ScriptableObjects/AbilityPower/{fileName}_AbilityPower_Level{j+1}.asset");
                if (AbilityPower[j] == null)
                {
                    Debug.Log(power);
                    AbilityPower[j] = CreateInstance(power) as AbilityPower;
                    AssetDatabase.CreateAsset(AbilityPower[j], $"Assets/BaseGame/ScriptableObjects/AbilityPower/{fileName}_AbilityPower_Level{j+1}.asset");
                }
                AbilityPower[j] = AssetDatabase.LoadAssetAtPath<AbilityPower>($"Assets/BaseGame/ScriptableObjects/AbilityPower/{fileName}_AbilityPower_Level{j+1}.asset");
                AbilityPower[j].UpdateData(database[i+j]);
            }
            
            AssetDatabase.RenameAsset(path, fileName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif

}