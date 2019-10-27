using System;
using System.Collections.Generic;
using System.Linq;
using LotteryV3.Domain.Extensions;

namespace LotteryV3.Domain.Entities
{
    public class NumberModel : Number
    {
        public int TotalSum;
        public double AvgSum;
        public int MinSum;
        public int MaxSum;

        public double SumSTD;
        public double SumVariance;

        public PropabilityGroup Group;

        public int TimesChosen => DrawingDates.Count;
        public double PercentChosen => ((double)TimesChosen / DrawingsCount) * 100;

        public decimal TrendlineYvalue { get; set; }

        public int DrawingsCount { get; private set; }
        public void SetDrawingsCount(int value) => DrawingsCount = value;

        public NumberModel(int id, int slotid, GameType game, DateTime firstDrawingDate) : base(id, slotid, game, firstDrawingDate) { }
        public void LoadDrawingsRange(List<Drawing> drawings, DateTime start, DateTime end)
        {
            LoadDrawings(drawings.Where(i => i.DrawingDate >= start && i.DrawingDate <= end).ToList());
        }
        public void LoadLastNumberOfDrawingsAndLeave(List<Drawing> drawings, int PreviousDrawingsCount, int LeaveDrawingCount)
        {
            int TakeCount = (PreviousDrawingsCount + LeaveDrawingCount) > drawings.Count ? drawings.Count - LeaveDrawingCount : PreviousDrawingsCount;

            LoadDrawings(drawings.OrderByDescending(i => i.DrawingDate).Take(TakeCount + LeaveDrawingCount).Skip(LeaveDrawingCount).ToList());
        }
        /// <summary>
        /// add all drawing dates when number was selected in given slot.
        /// </summary>
        /// <param name="drawings">list of drawings to define analysis pool.</param>
        public void LoadDrawings(List<Drawing> drawings)
        {
            var list = drawings.Where(drawing => drawing.Game == Game && drawing.Numbers[SlotId - 1] == Id).ToArray();
            foreach (var item in list)
            {
                base.AddDrawingDate(SlotId, item.Numbers[SlotId - 1], item.DrawingDate);
                TotalSum = +item.Sum;
            }
            DrawingsCount = drawings.Count;
            if (list.Count() == 0) return;
            MinSum = list.Select(i => i.Sum).Min();
            MaxSum = list.Select(i => i.Sum).Max();
            AvgSum = list.Select(i => i.Sum).Average();
            SumSTD = list.Select(i => i.Sum).ToList().StandardDeviation();
            SumVariance = list.Select(i => i.Sum).ToList().Variance();

        }
    }


}
