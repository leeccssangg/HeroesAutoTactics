namespace Combat
{
    public enum Trigger
    {
        None = 0,
        
        Sell = 1,
        Buy = 2,
        BuyAndSell = 3,
        Roll = 4,
        AllySold = 5,
        
        
        Faint = 10,
        AllyFaint = 11,
        EnemyFaint = 12,
        AnyFaint = 33,
        
        StartOfBattle = 20,
        EndOfBattle = 21,
        
        AllySummon = 30,
        Summoned = 31,
        
        Hurt = 40,
        AnyAllyHurt = 41,
        AllyAheadHurt = 42,
        
        BeforeAttack = 50,
        AfterAttack = 51,
        AllyAttack = 52,
        EnemyAttack = 53,
        
        
        EndTurn = 60,
        StartTurn = 61,
        
        GainStat = 70,
        AllyGainStat = 71,
        EnemyGainStat = 72,
        AnyGainStat = 73,
        
        LevelUp = 80,
        AllyLevelUp = 81,
    }
}