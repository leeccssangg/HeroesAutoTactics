public enum Targeting
{
    None = 0,
    RandomOpponent = 1,
    AllOpponent = 2,
    Self = 3,
    RandomAlly = 4,
    AllAlly = 5,
    AllAllyAndSelf = 6,
    Everyone = 7,
    FirstOpponent = 8,
    AllStoreHero = 9,
    RandomStoreHeroWithFaintPassive = 10,
    NearHeroAHead = 11,
    NearHeroBehind = 12,
}

public static class TargetingExtension
{
    public static string GetExplanation(this Targeting targeting)
    {
        return targeting switch
        {
            Targeting.None => "No Explanation",
            Targeting.RandomOpponent => "Random {Value0} Opponent",
            Targeting.AllOpponent => "All Opponent",
            Targeting.Self => "Self",
            Targeting.RandomAlly => "Random {Value0} Ally",
            Targeting.AllAlly => "All Ally",
            Targeting.AllAllyAndSelf => "All Ally and Self",
            Targeting.Everyone => "Everyone",
            Targeting.FirstOpponent => "First Opponent",
            Targeting.AllStoreHero => "All Store Hero",
            Targeting.RandomStoreHeroWithFaintPassive => "Random {Value0} Store Hero With Faint Passive",
            Targeting.NearHeroAHead => "{Value0} Near Hero AHead",
            Targeting.NearHeroBehind => "{Value0} Near Hero Behind",

            _ => "No Explanation"
        };
    }
}