using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Core;
using Game.Manager;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Combat
{
    [InlineEditor]
    public abstract class AbilityPower : ScriptableObject
    {
        protected InGameHandleManager InGameHandleManager { get; set; }
        protected FactoryManager FactoryManager { get; set; }
        
        [field: SerializeField] public int AbilityLevel {get; set;}
        [field: SerializeField] public string Description {get; set;}
        [field: SerializeField] public AbilityTrigger AbilityTrigger {get; set;}
        [field: SerializeField] public AbilityTarget AbilityTarget {get; set;}

        public virtual AbilityPower UpdateData(Dictionary<string, string> data)
        {
            EditorUtility.SetDirty(this);
            if (int.TryParse(data["AbilityLevel"], out int abilityLevel))
            {
                AbilityLevel = abilityLevel;
            }

            try
            {
                AbilityTrigger = AssetDatabase.LoadAssetAtPath<AbilityTrigger>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:AbilityTrigger {data["AbilityTrigger"]}")[0]));
            }
            catch (Exception e)
            {
                AbilityTrigger = null;
                Debug.Log($"AbilityTrigger {data["AbilityTrigger"]} not found");
                string trigger = data["AbilityTrigger"];
                AbilityTrigger abilityTrigger = CreateInstance(trigger) as AbilityTrigger;
                AssetDatabase.CreateAsset(abilityTrigger, $"Assets/BaseGame/ScriptableObjects/AbilityTrigger/{trigger}.asset");
                AbilityTarget = AssetDatabase.LoadAssetAtPath<AbilityTarget>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:AbilityTarget {data["AbilityTarget"]}")[0]));
            }

            try
            {
                AbilityTarget = AssetDatabase.LoadAssetAtPath<AbilityTarget>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets($"t:AbilityTarget {data["AbilityTarget"]}")[0]));
            }
            catch (Exception e)
            {
                AbilityTarget = null;
                Debug.Log($"AbilityTarget {data["AbilityTarget"]} not found");
            }

            Description = data["Description"];
            return this;
        }
        public abstract UniTask Resolve(Hero hero, TickRate tickRate, CancellationToken cancellationToken);
        public bool HasTrigger<T>() where T : AbilityTrigger
        {
            // Debug.Log(typeof(T) == AbilityTrigger.GetType());
            // Debug.Log(typeof(T) + "-" + AbilityTrigger.GetType());
            return typeof(T) == AbilityTrigger.GetType();
        }

        public AbilityPower Initialize(InGameHandleManager inGameHandleManager, FactoryManager factoryManager)
        {
            InGameHandleManager = inGameHandleManager;
            FactoryManager = factoryManager;
            
            AbilityTarget.Initialize(inGameHandleManager);
            return this;
        }
    }
}