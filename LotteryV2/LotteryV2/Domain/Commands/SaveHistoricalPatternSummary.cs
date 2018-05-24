using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace LotteryV2.Domain.Commands
{
    public class SaveHistoricalPatternSummary : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            SaveToCsvFile(context);
        }

        public void SaveToCsvFile(DrawingContext context)
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder sbWeighted = new StringBuilder();
            sbWeighted.AppendLine("Period, Days, Slot, Subset, times chosen, percent chosen");
            foreach (HistoricalPeriods period in Enum.GetValues(typeof(HistoricalPeriods)))
            {
                UniqueFingerPrints data = new UniqueFingerPrints(context, period);
                List<FingerPrint> values = data.Top60Percent();
                // Period, Slot, SubSet, Times applied.
                Dictionary<int, Dictionary<SubSets, int>> weightedslotes = data.WeightedSubSets(values);
                foreach (var slotid in weightedslotes.Keys)
                {
                    int totaldraws = weightedslotes[slotid].Values.Sum();
                    foreach (var subset in (SubSets[])Enum.GetValues(typeof(SubSets)))
                    {
                        sbWeighted.Append($"{period}, {(int)period}, {slotid}, {subset}, {weightedslotes[slotid][subset]}")
                            .AppendLine($", {(totaldraws != 0 ? ((weightedslotes[slotid][subset] / totaldraws) * 100) : 0)}%");
                    }

                }

                sbWeighted.Append(data.ToString());
            }

            System.IO.File.WriteAllText(context.FileHistoricalSummary, sbWeighted.ToString());
        }
    }
}
