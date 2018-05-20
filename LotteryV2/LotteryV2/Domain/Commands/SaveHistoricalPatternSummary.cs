using System.Collections.Generic;
using System.Text;
using System;

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

            foreach (HistoricalPeriods period in Enum.GetValues(typeof(HistoricalPeriods)))
            {
                UniqueFingerPrints data = new UniqueFingerPrints(context, period);
                List<FingerPrint> values = data.Top60Percent();
                Dictionary<int, Dictionary<SubSets, int>> weightedslotes = data.WeightedSubSets(values);
                sb.Append(data.ToString());
            }

            System.IO.File.WriteAllText(context.FileHistoricalSummary, sb.ToString());
        }
    }
}
