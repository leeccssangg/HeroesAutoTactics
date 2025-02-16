using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat
{
    [Serializable, HideLabel]
    public class TargetValue
    {
        [field: SerializeField, HideInInspector] public Targeting Targeting {get; set;}
        [field: SerializeField, HideInInspector] public Effect.Type EffectType {get; set;}
        [field: ShowIf("@ShowEffectValue(0)")]
        [field: SerializeField] public int Value0 {get; set;}
        [field: ShowIf("@ShowEffectValue(1)")]
        [field: SerializeField] public int Value1{get; set;}
        [field: ShowIf("@ShowEffectValue(2)")]
        [field: SerializeField] public int Value2{get; set;}
        [field: ShowIf("@ShowEffectValue(3)")]
        [field: SerializeField] public int Value3{get; set;}
        [field: ShowIf("@ShowEffectValue(4)")]
        [field: SerializeField] public int Value4{get; set;}

            
        public bool ShowEffectValue(int index)
        {
            return GetTargetCost() >= index; 
        }

        private int GetTargetCost()
        {
            return Targeting switch
            {
                Targeting.None => -1,
                Targeting.RandomOpponent => 0,
                Targeting.AllOpponent => -1,
                Targeting.Self => -1,
                Targeting.RandomAlly => 0,
                Targeting.AllAlly => -1,
                Targeting.AllAllyAndSelf => -1,
                Targeting.Everyone => -1,
                Targeting.FirstOpponent => -1,
                Targeting.AllStoreHero => -1,
                Targeting.RandomStoreHeroWithFaintPassive => 0,
                Targeting.NearHeroAHead => 0,
                Targeting.NearHeroBehind => 0,
                _ => 0
            };
        }
    }
}