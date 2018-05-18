using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System;

namespace LotteryV2.Domain.Commands
{
    public class LoadFilehistoricalPeriods : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return System.IO.File.Exists(context.FileHistoricalPeriods);
        }

        public override void Execute(DrawingContext context)
        {
            LoadFromFile(context);
        }

        private void LoadFromFile(DrawingContext context)
        {

            List<HistoricalPeriodsJson> results = JsonConvert.DeserializeObject<List<HistoricalPeriodsJson>>
                (System.IO.File.ReadAllText(context.FileHistoricalPeriods));
            foreach (var item in results)
            {
                if (item.JsonHistoricalFingerPrints != null)
                {
                    Drawing drawing = context.AllDrawings.FirstOrDefault(d => d.DrawingDate == item.DrawingDate);

                    if (drawing != null)
                    {
                        drawing.JsonHistoricalFingerPrints = item.JsonHistoricalFingerPrints;
                        drawing.HistoricalPeriodFingerPrints.Select(i => i).ToList()
                            .ForEach(j => j.Value.DrawingDates.Add(drawing.DrawingDate));
                    }
                }
            }
        }
    }

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
