using System;
using LotteryV2.Domain.Extensions;

namespace LotteryV2.Domain.Model
{
    public class GetTimesChosenInDateRangeItem
    {
        public DateTime StartDate { get; private set; }
        public int Period { get; private set; }
        public int BallId { get; private set; }
        public int SlotId { get; private set; }
        public int Count { get; private set; }
        public string Percent { get; private set; }
        public  Game Game { get; private set; }
        public GetTimesChosenInDateRangeItem(BallTimesChosenInPeriodsDataSetItem item, DateTime startDate, int slotId, int period, string game)
        {
            BallId = item.BallId;
            Count = item.Count;
            Percent = item.Percent;
            StartDate = startDate;
            Period = period;
            SlotId = slotId;
            Game = OtherExtensions.ToEnum<Game>(game, Game.Match4);
        }

    }
}
