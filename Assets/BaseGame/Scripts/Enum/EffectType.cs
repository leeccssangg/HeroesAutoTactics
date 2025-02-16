using UnityEngine;

namespace Combat
{
    public partial class Effect
    {
        public enum Type
        {
            None = 0,
            DealDamage = 1,
            GiveStat = 2,
            GiveStatus = 3,
            GiveCoin = 4,
            GiveExp = 5,
            Summon = 6,
            SummonEnemy = 7,
            Transform = 8,
            GiveFreeRoll = 9,
            SummonAndGiveExp = 10,
            GiveStatPerFiendHasFaintAbility = 11,
            SetStat = 12,
            
        }
    }
    
    public static class EffectTypeExtension
    {
        public static string GetExplanation(this Effect.Type effectType)
        {
            return effectType switch
            {
                Effect.Type.None => "No Explanation",
                Effect.Type.DealDamage => "Deal {Value5} damage to target {Value6} times",
                Effect.Type.GiveStat => "Give {Value5} Attack and {Value6} Health to target",
                Effect.Type.GiveStatus => "Give status {Value5} with {Value6} stack to target",
                Effect.Type.GiveCoin => "Give {Value5} coin",
                Effect.Type.GiveExp => "Give {Value5} exp",
                Effect.Type.Summon => "Summon amount {Value5} unit {Value6} level {Value7} with {Value8} attack and {Value9} health",
                Effect.Type.SummonEnemy => "Summon amount {Value5} enemy unit {Value6} level {Value7} with {Value8} attack and {Value9} health",
                Effect.Type.Transform => "Transform target to unit {Value5} with +{Value6} attack and +{Value7} health",
                Effect.Type.GiveFreeRoll => "Give {Value5} free roll",
                Effect.Type.SummonAndGiveExp => "Summon amount {Value5} unit {Value6} level {Value7} with {Value8} attack and {Value9} health and give {Value10} exp",
                Effect.Type.GiveStatPerFiendHasFaintAbility => "Give {Value5} Attack and {Value6} Health to target per fiend has faint ability",
                _ => "No Explanation"
            };
        }
    }
}