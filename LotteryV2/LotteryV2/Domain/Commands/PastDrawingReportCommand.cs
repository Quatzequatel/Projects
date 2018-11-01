using System.Linq;
using System.Text;
using System;

namespace LotteryV2.Domain.Commands
{
    public class PastDrawingReportCommand : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            Console.WriteLine("PastDrawingReportCommand");
            return context.Drawings.Count > 0;
        }

        public override void Execute(DrawingContext context)
        {
            int AllDrawingsCount = context.AllDrawings.Count;
            int SampleSize = context.SampleSize;
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"{context.Drawings[0].HeadingCSVShort()}, {String.Join(",", Enum.GetNames(typeof(HistoricalPeriods)).Select(i => $"{i}, {i}Value"))}");
            foreach (var item in context.AllDrawings.Skip(AllDrawingsCount - SampleSize).Take(SampleSize))
            {
                sb.Append($"{item.ToCSVShort()}");
                foreach (var period in ((HistoricalPeriods[])Enum.GetValues(typeof(HistoricalPeriods))))
                {
                    if (item.HistoricalPeriodFingerPrints.ContainsKey(period))
                    {
                        sb.Append($", {item.HistoricalPeriodFingerPrints[period].ToString()}");
                    }
                }
                sb.AppendLine();
            }
            System.IO.File.WriteAllText($"{context.FilePath}{context.GetGameName()}-PastDrawingsReport.csv", sb.ToString());
        }
    }
}
