using UnityEngine;

namespace Combat
{
    public enum LevelScaleMethod
    {
        Linear = 0,
        Exponential = 1,
        Logarithmic = 2,
        Sqrt = 3,
        Cubic = 4,
    }
    
    public static class LevelScaleMethodExtension
    {
        public static int CalculateScale(this LevelScaleMethod levelScaleMethod, int level, float e1, float e2, float e3, float e4)
        {
            switch (levelScaleMethod)
            {
                case LevelScaleMethod.Linear:
                    return (int)(level * e1 + e2);
                case LevelScaleMethod.Exponential:
                    return (int)(e1 * Mathf.Pow(level, e2) + e3);
                case LevelScaleMethod.Logarithmic:
                    return (int)(e1 * Mathf.Log(level + e2) + e3);
                case LevelScaleMethod.Sqrt:
                    return (int)(e1 * Mathf.Sqrt(level) + e2);
                case LevelScaleMethod.Cubic:
                    return (int)(e1 * Mathf.Pow(level, 3) + e2 * Mathf.Pow(level, 2) + e3 * level + e4);
                default:
                    return level;
            }
        }
        public static string GetExplanation(this LevelScaleMethod levelScaleMethod, float e1, float e2, float e3, float e4)
        {
            switch (levelScaleMethod)
            {
                case LevelScaleMethod.Linear:
                    return $"value = {e1} * level + {e2}";
                case LevelScaleMethod.Exponential:
                    return $"value = {e1} * level^{e2} + {e3}";
                case LevelScaleMethod.Logarithmic:
                    return $"value = {e1} * log(level + {e2}) + {e3}";
                case LevelScaleMethod.Sqrt:
                    return $"value = {e1} * sqrt(level) + {e2}";
                case LevelScaleMethod.Cubic:
                    return $"value = {e1} * level^3 + {e2} * level^2 + {e3} * x + {e4}";
                default:
                    return "value = level";
            }
        }
        public static string GetExplanation(this LevelScaleMethod levelScaleMethod)
        {
            switch (levelScaleMethod)
            {
                case LevelScaleMethod.Linear:
                    return "value = e1 * level + e2";
                case LevelScaleMethod.Exponential:
                    return "value = e1 * level^e2 + e3";
                case LevelScaleMethod.Logarithmic:
                    return "value = e1 * log(level + e2) + e3";
                case LevelScaleMethod.Sqrt:
                    return "value = e1 * sqrt(level) + e2";
                case LevelScaleMethod.Cubic:
                    return "value = e1 * level^3 + e2 * level^2 + e3 * x + e4";
                default:
                    return "value = level";
            }
        }
    }
}