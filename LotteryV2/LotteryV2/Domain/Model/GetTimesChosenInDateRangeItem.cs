using System;
using LotteryV2.Domain.Extensions;

namespace LotteryV2.Domain.Model
{
    public class GetTimesChosenInDateRangeItem
    {
        public DateTime EndPeriodDate { get; private set; }
        public int TestId { get; private set; }
        public int Period { get; private set; }
        public int BallId { get; private set; }
        public int SlotId { get; private set; }
        public int Count { get; private set; }
        public string Percent { get; private set; }
        public  Game Game { get; private set; }
        public GetTimesChosenInDateRangeItem(BallTimesChosenInPeriodsDataSetItem item, int testId, DateTime startPeriodDate, int slotId, int period, string game)
        {
            TestId = testId;
            BallId = item.BallId;
            Count = item.Count;
            Percent = item.Percent;
            EndPeriodDate = startPeriodDate.AddDays(period);
            Period = period;
            SlotId = slotId;
            Game = OtherExtensions.ToEnum<Game>(game, Game.Match4);
        }
    }
}
