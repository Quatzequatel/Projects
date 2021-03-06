﻿using System;
using System.Collections.Generic;
using System.Linq;
using LotteryV2.Domain.Extensions;


namespace LotteryV2.Domain.Model
{
    /// <summary>
    /// NumberModel builds on Number to add a few statics data.
    /// Sum, Min/Max Sum, Avg Sum,
    /// </summary>
    public class NumberModel : Number
    {
        public NumberModel(int id, int slotid, Game game) : base(id, slotid, game) { }
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
            var list = drawings.Where(drawing => drawing.Game == Game && drawing.Numbers[SlotId - 1] == BallId).ToArray();
            foreach (var item in list)
            {
                base.AddDrawingDate(item.DrawingDate);
                TotalSum += item.Sum;
            }
            DrawingsCount = drawings.Count;
            if (list.Count() == 0) return;
            MinSum = list.Select(i => i.Sum).Min();
            MaxSum = list.Select(i => i.Sum).Max();
            AvgSum = list.Select(i => i.Sum).Average();
            SumSTD = list.Select(i => i.Sum).ToList().StandardDeviation();
            SumVariance = list.Select(i => i.Sum).ToList().Variance();

        }

        public int TotalSum;
        public double AvgSum;
        public int MinSum;
        public int MaxSum;

        public double SumSTD;
        public double SumVariance;

        //public SlotGroup Group;

        public int TimesChosen => DrawingDates.Count;
        public double PercentChosen => ((double)TimesChosen / DrawingsCount) * 100;

        /// <summary>
        /// The number of drawings in the data set.
        /// </summary>
        public int DrawingsCount { get; private set; }

        /// <summary>
        /// Set the DrawingsCount value.
        /// </summary>
        /// <param name="value"></param>
        public void SetDrawingsCount(int value) => DrawingsCount = value;

        /// <summary>
        /// 
        /// </summary>
        public string CSVHeading => new string[] { "Game", "Slot", "Number", "Times Chosen", "Chosen %", "AvgSum", "MinSum", "MaxSum","SumSTD","SumVariance" }.CSV();
        public string CSVLine => new string[]
        {
            $"{Game}",
            $"{SlotId}",
            $"{BallId}",
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
