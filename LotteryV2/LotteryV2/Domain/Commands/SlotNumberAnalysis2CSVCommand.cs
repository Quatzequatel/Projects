using System;
using System.Collections.Generic;
using System.Text;
using LotteryV2.Domain.Model;

namespace LotteryV2.Domain.Commands
{
    public class SlotNumberAnalysis2CSVCommand : Command<DrawingContext>
    {
        public string Filename { get; set; }
        private List<NumberModel> numbers = new List<NumberModel>();
        Dictionary<int, SlotGroup> groups;

        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SlotNumberAnalysis2CSVCommand");
            Filename = Filename = $"{context.FilePath}{context.GetGameName()}_SlotNumberAnalysis.csv";
            numbers = context.NumberModelList;
            groups = context.GroupsDictionary;
            SaveToCSV(context);
        }

        private void SaveToCSV(DrawingContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(numbers[0].CSVHeading + ",GroupType");
            foreach (var item in numbers)
            {
                sb.AppendLine(item.CSVLine + $",{groups[item.SlotId].FindGroupType(item.BallId)}");
            }

            System.IO.File.WriteAllText(Filename, sb.ToString());
        }
    }
}
