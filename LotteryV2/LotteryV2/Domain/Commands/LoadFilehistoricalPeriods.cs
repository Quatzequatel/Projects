using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace LotteryV2.Domain.Commands
{
    public class LoadFilehistoricalPeriods : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            LoadFromFile(context);
        }

        private void LoadFromFile(DrawingContext context)
        {

            List<HistoricalPeriodsJson> results = JsonConvert.DeserializeObject<List<HistoricalPeriodsJson>>(System.IO.File.ReadAllText(context.FilehistoricalPeriods));
            foreach (var item in results)
            {
                if (item.JsonHistoricalFingerPrints != null)
                {
                    Drawing drawing = context.AllDrawings.FirstOrDefault(d => d.DrawingDate == item.DrawingDate);

                    if (drawing != null) drawing.JsonHistoricalFingerPrints = item.JsonHistoricalFingerPrints;
                }
            }
        }
    }
}
