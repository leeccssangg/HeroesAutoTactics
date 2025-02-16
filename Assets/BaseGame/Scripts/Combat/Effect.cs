using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat
{
    [Serializable]
    public partial class Effect
    {
        [field: SerializeField] public Targeting Targeting { get; private set; }
        [field: SerializeField, TableList (AlwaysExpanded = true)] 
        public TargetValue[] TargetValue {get; set;} = new TargetValue[3];
        [field: SerializeField] public Type EffectType { get; private set; }
        
        [field: SerializeField, TableList (AlwaysExpanded = true)] 
        public EffectValue[] EffectValue {get; set;} = new EffectValue[3];

        [field: SerializeField] public Projectile Projectile { get; private set; }
        
#if UNITY_EDITOR
        [ShowInInspector]
        private string TargetExplanation  => Targeting.GetExplanation();
        [ShowInInspector]
        private string EffectExplanation => EffectType.GetExplanation();
#endif

        public EffectValue GetValue(int level)
        {       
            return EffectValue[level - 1];
        }
        public TargetValue GetTargetValue(int level)
        {
            return TargetValue[level - 1];
        }

#if UNITY_EDITOR
        [OnInspectorGUI]
        private void OnInspectorGUI()
        {
            foreach (EffectValue effectValue in EffectValue)
            {
                effectValue.EffectType = EffectType;
            }

            foreach (TargetValue targetValue in TargetValue)
            {
                targetValue.Targeting = Targeting;
            }
        }
#endif
    }

}