using System;
using System.Collections.Generic;
using System.Linq;
using LotteryV3.Domain.Extensions;

namespace LotteryV3.Domain.Entities
{
    /// <summary>
    /// NumberInfo builds on number storing more analysis data on a given slot:ball pair and can hold data in 
    /// relationship to other numbers for given slot.
    /// </summary>
    public class NumberInfo : Number
    {
        public int TotalSum;
        public double AvgSum;
        public int MinSum;
        public int MaxSum;

        public double SumSTD;
        public double SumVariance;

        public PropabilityType GroupType;

        public int TimesChosen => DrawingDates.Count;
        public int DrawingsCount { get; private set; }
        public void SetDrawingsCount(int value) => DrawingsCount = value;

        public double PercentChosen => ((double)TimesChosen / DrawingsCount) * 100;

        public NumberInfo(int Id, int slotId, GameType game, DateTime firstDrawingDate) : base(Id, slotId, game, firstDrawingDate)
        {

        }

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



        public string CSVHeading => new string[] { "Game", "Slot", "Number", "Times Chosen", "Chosen %", "AvgSum", "MinSum", "MaxSum", "SumSTD", "SumVariance" }.CSV();
        public string CSVLine => new string[]
        {
            $"{Game}",
            $"{SlotId}",
            $"{Id}",
            $"{TimesChosen}",
            $"{PercentChosen}",
            $"{AvgSum}",
            $"{MinSum}",
            $"{MaxSum}",
            $"{SumSTD}",
            $"{SumVariance}"
        }.CSV();
    }


}
