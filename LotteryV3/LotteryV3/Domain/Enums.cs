namespace LotteryV3.Domain
{
    public enum PropabilityType
    {
        Zero,
        Low,
        MidLow,
        Mid,
        MidHigh,
        High
    }
    public enum TrendType
    {
        Zero,
        LowAndDecreasing,
        Low,
        LowAndIncreasing,
        HighAndDecreasing,
        High,
        HighAndIncreasing
    }
    public enum GameType
    {
        Lotto,
        MegaMillion,
        Powerball,
        Hit5,
        Match4
    }
}
