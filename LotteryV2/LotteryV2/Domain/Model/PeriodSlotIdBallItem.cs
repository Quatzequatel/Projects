using System;
using LotteryV2.Domain.Extensions;


namespace LotteryV2.Domain.Model
{
    public class PeriodSlotIdBallItem
    {
        public DateTime EndPeriodDate { get; private set; }
        public int TestId { get; private set; }
        public int Period { get; private set; }
        public int BallId { get; private set; }
        public int SlotId { get; private set; }
        public int Count { get; private set; }
        public Game Game { get; private set; }

        public PeriodSlotIdBallItem(DateTime endPeriodDate, int count)
        {
            EndPeriodDate = endPeriodDate;
            Count = count;
        }

        public PeriodSlotIdBallItem(DateTime endPeriodDate, int count, int testId, int period, int ballId, int slotId, String game)
            :this(endPeriodDate,count)
        {
            Update(testId, period, ballId, slotId, game);
        }

        public void Update(int testId, int period, int ballId, int slotId, string game)
        {
            TestId = testId;
            Period = period;
            BallId = ballId;
            SlotId = slotId;
            Game = OtherExtensions.ToEnum<Game>(game, Game.Match4);
        }

        public double CountToDouble() => Convert.ToDouble(this.Count);
        public double EndPeriodDateToDouble() => this.EndPeriodDate.ToOADate();
    }

    public class InsertSlopeInterceptDetailsItem
    {
        public DateTime EndPeriodDate { get; private set; }
        //public int TestId { get; private set; }
        public int Period { get; private set; }
        public int BallId { get; private set; }
        public int SlotId { get; private set; }
        public double Intercept { get; private set; }
        public double Slope { get; private set; }
        public int? NextTimeChosenCount { get; private set; }
        public Game Game { get; private set; }
        public InsertSlopeInterceptDetailsItem(DateTime endPeriodDate, int period, int ballId, int slotId,
            double intercept, double slope, int? nextTimeChosenCount, String game)
        {
            EndPeriodDate = endPeriodDate;
            //TestId = testId;
            Period = period;
            BallId = ballId;
            SlotId = slotId;
            Intercept = intercept;
            Slope = slope;
            NextTimeChosenCount = nextTimeChosenCount;

            Game = OtherExtensions.ToEnum<Game>(game, Game.Match4);

        }
    }
}
