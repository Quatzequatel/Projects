using LotteryV2.Domain.Commands;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotteryV2.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public class UniqueFingerPrints
    {
        /// <summary>
        /// int == FingerPrint.Value; a unique int key of the fingerprint.
        /// </summary>
        public Dictionary<int, FingerPrint> FingerPrints { get; set; } = new Dictionary<int, FingerPrint>();

        /// <summary>
        /// Setter only to be used by Json deserializer.
        /// </summary>
        public HistoricalPeriods Period { get; set; }

        public int TotalChosen() => FingerPrints.Sum(i => i.Value.TimesChoosen);

        /// <summary>
        /// should only be used by Json deserializer.
        /// </summary>
        public UniqueFingerPrints() { }

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
        /// <summary>
        /// Returns the list of fingerprints that represent the top 60% choosen.
        /// </summary>
        /// <returns></returns>
        public List<FingerPrint> Top60Percent()
        {
            List<FingerPrint> results = new List<FingerPrint>();
            //int total = TotalChosen();
            int sixtyPercent = (int)Math.Ceiling(TotalChosen() * 0.6);
            int currentTotal = 0;
            foreach (var finger in FingerPrints.OrderByDescending(i => i.Value.TimesChoosen).ToList())
            {
                if (currentTotal <= sixtyPercent)
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
        /// Where the weight is the number of times subset was choosen.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public Dictionary<int, Dictionary<SubSets, int>> WeightedSubSets(List<FingerPrint> values)
        {
            Dictionary<int, Dictionary<SubSets, int>> slots = InitailizeSlots();
            int slotcount = new int[0].GetSlotCount();

            foreach (var finger in values)
            {
                for (int slot = 0; slot < slotcount; slot++)
                {
                    SubSets set = finger.GetTemplate()[slot];
                    slots[slot][set] += finger.TimesChoosen;
                }
            }

            return slots;
        }

        /// <summary>
        /// Creates an outer Dictionary from 0 to SlotCount.
        /// Initialize each with an inner Dictionary of SubSets with value = 0;
        /// </summary>
        /// <returns></returns>
        private Dictionary<int, Dictionary<SubSets, int>> InitailizeSlots()
        {
            Dictionary<int, Dictionary<SubSets, int>> result = new Dictionary<int, Dictionary<SubSets, int>>();

            for (int i = 0; i < new int[0].GetSlotCount(); i++)
            {
                result[i] = new Dictionary<SubSets, int>();
                foreach (var set in (SubSets[])Enum.GetValues(typeof(SubSets)))
                {
                    result[i][set] = 0;
                }
            }

            return result;
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

    public class SlotNumberSubSet
    {
        public SlotNumberSubSet()
        {

        }

        public Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> Data;
        //public 

    }
}
