using LotteryV2.Domain.Commands;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotteryV2.Domain
{
    public class UniqueFingerPrints
    {
        public Dictionary<int, FingerPrint> FingerPrints { get; set; } = new Dictionary<int, FingerPrint>();

        public HistoricalPeriods Period { get; set; }

        public int TotalChosen() => FingerPrints.Sum(i => i.Value.TimesChoosen);

        public UniqueFingerPrints()
        {

        }

        public UniqueFingerPrints(DrawingContext context, HistoricalPeriods period)
        {
            Period = period;
            foreach (var drawing in context.AllDrawings)
            {
                if (drawing.HistoricalPeriodFingerPrints.ContainsKey(period))
                {
                    AddFingerPrint(drawing.HistoricalPeriodFingerPrints[period]);
                }
            }
        }

        public void AddFingerPrint(FingerPrint fingerPrint)
        {
            if (FingerPrints.ContainsKey(fingerPrint.Value))
            {
                FingerPrints[fingerPrint.Value].DrawingDates.Add(fingerPrint.DrawingDates.First());
                return;
            }

            FingerPrints[fingerPrint.Value] = fingerPrint.Clone();
        }

        public List<FingerPrint> Top60Percent()
        {
            List<FingerPrint> results = new List<FingerPrint>();
            //int total = TotalChosen();
            int sixtyPercent = (int)Math.Ceiling(TotalChosen() * 0.6);
            int currentTotal = 0;
            foreach (var finger in FingerPrints.OrderByDescending(i => i.Value.TimesChoosen).ToList())
            {
                if( currentTotal <= sixtyPercent)
                {
                    results.Add(finger.Value);
                    currentTotal += finger.Value.TimesChoosen;
                }
            }

            return results;
        }
        /// <summary>
        /// Retuns a dictionary of dictionaies.
        /// the outer dictionary represents each slot.
        /// the inner dictionary represens the weighted values of each subset.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public Dictionary<int, Dictionary<SubSets, int>> WeightedSubSets(List<FingerPrint> values)
        {
            Dictionary<int, Dictionary<SubSets, int>> slots = new Dictionary<int, Dictionary<SubSets, int>>();
            int slotcount = new int[0].GetSlotCount();
            //todo initalize the dictionaries

            foreach (var finger in values)
            {
                for (int slot = 0; slot < slotcount; slot++)
                {
                    slots[slot][finger.GetTemplate()[slot]] += finger.Value;
                }
            }

            return slots;
        }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            foreach (var finger in FingerPrints.OrderByDescending(i => i.Value.TimesChoosen).ToList())
            {
                sb.Append(Period.GetName(typeof(HistoricalPeriods))).Append(",").AppendLine(finger.ToString());
            }

            return sb.ToString();
        }
    }
}
