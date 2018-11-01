using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
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
            Console.WriteLine("LoadFilehistoricalPeriods");
            LoadFromFile(context);
            context.ShouldExecuteSetHistoricalPeriods = false;
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
}
