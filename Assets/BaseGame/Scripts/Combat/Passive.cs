using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat
{
    [InlineEditor]
    [CreateAssetMenu (fileName = "Passive", menuName = "ScriptableObjects/Combat/Passive")]
    public class Passive : ScriptableObject
    {
        [field: SerializeField] public Trigger Trigger { get; set; }

        [field: Title(nameof(Effect))]
        [field: SerializeField, HideLabel] public Effect Effect { get; set; }
        
        [field: SerializeField, TextArea(3, 6)]
        public string PassiveDescription { get; set; }
        public string GetDescription(int level)
        {
            EffectValue effectValue = Effect.GetValue(level);
            TargetValue targetValue = Effect.GetTargetValue(level);
            return string.Format(PassiveDescription, 
                targetValue.Value0.ToString(),
                targetValue.Value1.ToString(),
                targetValue.Value2.ToString(),
                targetValue.Value3.ToString(),
                targetValue.Value4.ToString(),
                effectValue.Value5.ToString(), 
                effectValue.Value6.ToString(), 
                effectValue.Value7.ToString(), 
                effectValue.Value8.ToString(),
                effectValue.Value9.ToString(),
                effectValue.Value10.ToString());
        }
    }
}