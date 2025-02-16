using System;
using System.Collections.Generic;
using System.Linq;
using Combat;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.GlobalConfig;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using TW.Utility.Extension;

#if UNITY_EDITOR
using Spine.Unity;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HeroGenerateGlobalConfig", menuName = "GlobalConfigs/HeroGenerateGlobalConfig")]
[GlobalConfig("Assets/Resources/GlobalConfig/")]
public class HeroGenerateGlobalConfig : GlobalConfig<HeroGenerateGlobalConfig>
{
    [field: SerializeField] private string IdDataBase { get; set; }
    [field: SerializeField] public Hero HeroBasePrefab { get; private set; }
    [field: SerializeField] public List<HeroConfigData> HeroConfigDataList { get; private set; }
    [field: SerializeField] public List<Hero> HeroPrefabList { get; private set; }
    
    public async UniTask GenerateAllHeroConfigData()
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        HeroConfigDataList = AssetDatabase.FindAssets("t:HeroConfigData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroConfigData>)
            .ToList();
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/BaseGame/Prefabs/Hero" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();

        string rawData = await ABakingSheet.GetCsv(IdDataBase, "HeroDataBase");
        List<Dictionary<string, string>> database = ACsvReader.ReadDataFromString(rawData);
        foreach (Dictionary<string, string> heroData in database)
        {
            string heroName = $"{heroData["ID"]}_{heroData["HeroName"]}";
            HeroConfigData heroConfigData = HeroConfigDataList.FirstOrDefault(cf => cf.HeroName == heroName);
            if (heroConfigData == null)
            {
                heroConfigData = CreateInstance<HeroConfigData>();
                heroConfigData.HeroName = heroName;
                HeroConfigDataList.Add(heroConfigData);

                AssetDatabase.CreateAsset(heroConfigData,
                    $"Assets/BaseGame/ScriptableObjects/HeroConfigData/{heroConfigData.HeroName}.asset");
                AssetDatabase.SaveAssets();
            }

            EditorUtility.SetDirty(heroConfigData);
            heroConfigData.HeroId = int.Parse(heroData["ID"]);
            heroConfigData.Rarity = (Rarity)Enum.Parse(typeof(Rarity), heroData["Rarity"]);
            heroConfigData.HealthPoint = int.Parse(heroData["HealthPoint"]);
            heroConfigData.AttackDamage = int.Parse(heroData["AttackDamage"]);
            try
            {
                string guid = AssetDatabase.FindAssets($"t:SkeletonDataAsset {heroData["HeroName"]}",
                    new string[] { "Assets/BaseGame/Graphics/Sprites/Hero" })[0];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                heroConfigData.SkeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(path);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            Hero heroPrefab = HeroPrefabList.FirstOrDefault(p => p.name == heroName);
            if (heroPrefab == null)
            {
                heroPrefab = Instantiate(HeroBasePrefab);
                heroPrefab.name = heroName;
                PrefabUtility.SaveAsPrefabAsset(heroPrefab.gameObject,
                    $"Assets/BaseGame/Prefabs/Hero/{heroPrefab.name}.prefab");
                AssetDatabase.SaveAssets();
                DestroyImmediate(heroPrefab.gameObject);
                heroPrefab = AssetDatabase.LoadAssetAtPath<Hero>($"Assets/BaseGame/Prefabs/Hero/{heroName}.prefab");
                HeroPrefabList.Add(heroPrefab);
            }

            EditorUtility.SetDirty(heroConfigData);
            heroConfigData.HeroPrefab = heroPrefab;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            using PrefabUtility.EditPrefabContentsScope scope =
                new PrefabUtility.EditPrefabContentsScope($"Assets/BaseGame/Prefabs/Hero/{heroName}.prefab");
            heroPrefab.SetHeroConfigData(heroConfigData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        HeroPoolGlobalConfig.Instance.GetAllHeroConfigData();
#endif
    }

    [Button]
    public async UniTask GenerateNewHeroConfigData(int tier)
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
        HeroConfigDataList = AssetDatabase.FindAssets("t:HeroConfigData")
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<HeroConfigData>)
            .ToList();
        HeroPrefabList = AssetDatabase.FindAssets("t:Prefab", new string[] { $"Assets/BaseGame/Prefabs/Hero/Tier{tier}" })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Hero>)
            .ToList();

        string rawData = await ABakingSheet.GetCsv(IdDataBase, $"Tier{tier}");
        List<Dictionary<string, string>> database = ACsvReader.ReadDataFromString(rawData);
        foreach (Dictionary<string, string> heroData in database)
        {
            string heroName = heroData["HeroName"];
            string assetName = $"{heroData["ID"]}_{heroData["HeroName"]}";
            HeroConfigData heroConfigData = HeroConfigDataList.FirstOrDefault(cf => cf.name == assetName);
            if (heroConfigData == null)
            {
                heroConfigData = CreateInstance<HeroConfigData>();
                heroConfigData.HeroName = heroName;
                HeroConfigDataList.Add(heroConfigData);


                AssetDatabase.CreateAsset(heroConfigData,
                    $"Assets/BaseGame/ScriptableObjects/HeroConfigData/Tier{tier}/{assetName}.asset");
                AssetDatabase.SaveAssets();
            }

            EditorUtility.SetDirty(heroConfigData);
            heroConfigData.HeroId = int.Parse(heroData["ID"]);
            heroConfigData.Rarity = (Rarity)tier;
            heroConfigData.HealthPoint = int.Parse(heroData["HealthPoint"]);
            heroConfigData.AttackDamage = int.Parse(heroData["AttackDamage"]);
            try
            {
                string guid = AssetDatabase.FindAssets($"t:SkeletonDataAsset {heroData["HeroName"]}",
                    new string[] { $"Assets/BaseGame/Graphics/Sprites/Hero/Tier{tier}" })[0];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                heroConfigData.SkeletonDataAsset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(path);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
            
            try
            {
                string guid = AssetDatabase.FindAssets($"t:Sprite {heroData["HeroName"]}",
                    new string[] { $"Assets/BaseGame/Graphics/Sprites/Hero/Tier{tier}" })[0];
                string path = AssetDatabase.GUIDToAssetPath(guid);
                heroConfigData.SpriteIcon = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            Hero heroPrefab = HeroPrefabList.FirstOrDefault(p => p.name == assetName);
            if (heroPrefab == null)
            {
                heroPrefab = Instantiate(HeroBasePrefab);
                heroPrefab.name = assetName;
                PrefabUtility.SaveAsPrefabAsset(heroPrefab.gameObject,
                    $"Assets/BaseGame/Prefabs/Hero/Tier{tier}/{assetName}.prefab");
                AssetDatabase.SaveAssets();
                DestroyImmediate(heroPrefab.gameObject);
                heroPrefab = AssetDatabase.LoadAssetAtPath<Hero>($"Assets/BaseGame/Prefabs/Hero/Tier{tier}/{assetName}.prefab");
                HeroPrefabList.Add(heroPrefab);
            }

            EditorUtility.SetDirty(heroConfigData);
            heroConfigData.HeroPrefab = heroPrefab;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            using PrefabUtility.EditPrefabContentsScope scope = new PrefabUtility.EditPrefabContentsScope($"Assets/BaseGame/Prefabs/Hero/Tier{tier}/{assetName}.prefab");
            scope.prefabContentsRoot.GetComponent<Hero>().SetHeroConfigData(heroConfigData);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        HeroPoolGlobalConfig.Instance.GetAllHeroConfigData();
#endif
    }
}