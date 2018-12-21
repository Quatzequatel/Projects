namespace LotteryV2.Domain.Model
{
    public class BallTimesChosenInPeriodsDataSetItem
    {
        public int BallId { get; private set; }
        public int Count { get; private set; }
        public string Percent { get; private set; }
        public BallTimesChosenInPeriodsDataSetItem(int ballId, int count,  string percent)
        {
            BallId = ballId;
            Count = count;
            Percent = percent;
        }
    }
}
