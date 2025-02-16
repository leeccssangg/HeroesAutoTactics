using System;

public enum Rank
{
    Iron,
    Bronze,
    Silver,
    Gold,
    Platinum,
    Diamond,
    Ascended,
    Immortal,
    Radiant,
}

public static class RankExtension
{
    public static string ToRoman(this Rank rank)
    {
        switch (rank)
        {
            case Rank.Iron:
                return "I";
            case Rank.Bronze:
                return "II";
            case Rank.Silver:
                return "III";
            case Rank.Gold:
                return "IV";
            case Rank.Platinum:
                return "V";
            case Rank.Diamond:
                return "VI";
            case Rank.Ascended:
                return "VII";
            case Rank.Immortal:
                return "VIII";
            case Rank.Radiant:
                return "IX";
            default:
                throw new ArgumentOutOfRangeException(nameof(rank), rank, null);
        }
        
    }
}