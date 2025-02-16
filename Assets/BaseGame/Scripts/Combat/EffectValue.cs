using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Combat
{
    [Serializable, HideLabel]
    public class EffectValue
    { 
        [field: SerializeField, HideInInspector] public Effect.Type EffectType {get; set;}
        [field: ShowIf("@ShowEffectValue(0)")]
        [field: SerializeField] public int Value5 {get; set;}
        [field: ShowIf("@ShowEffectValue(1)")]
        [field: SerializeField] public int Value6{get; set;}
        [field: ShowIf("@ShowEffectValue(2)")]
        [field: SerializeField] public int Value7{get; set;}
        [field: ShowIf("@ShowEffectValue(3)")]
        [field: SerializeField] public int Value8{get; set;}
        [field: ShowIf("@ShowEffectValue(4)")]
        [field: SerializeField] public int Value9{get; set;}
        [field: ShowIf("@ShowEffectValue(4)")]
        [field: SerializeField] public int Value10{get; set;}
        public bool ShowEffectValue(int index)
        {
            return GetEffectCost() >= index; 
        }

        private int GetEffectCost()
        {
            return EffectType switch
            {
                Effect.Type.None => -1,
                Effect.Type.DealDamage => 1,
                Effect.Type.GiveStat => 1,
                Effect.Type.GiveStatus => 1,
                Effect.Type.GiveCoin => 0,
                Effect.Type.GiveExp => 0,
                Effect.Type.Summon => 4,
                Effect.Type.SummonEnemy => 4,
                Effect.Type.Transform => 2,
                Effect.Type.GiveFreeRoll => 0,
                Effect.Type.SummonAndGiveExp => 5,
                Effect.Type.GiveStatPerFiendHasFaintAbility => 1,
                _ => -1
            };
        }
    }
}