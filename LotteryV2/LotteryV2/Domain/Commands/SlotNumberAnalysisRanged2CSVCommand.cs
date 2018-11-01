using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SlotNumberAnalysisRanged2CSVCommand : Command<DrawingContext>
    {
        public string Filename { get; set; }
        private List<NumberModel> numbers = new List<NumberModel>();
        public int TakeDrawwings { get; set; } = 1000;
        public int LeaveDrawings { get; set; } = 10;

        public override bool ShouldExecute(DrawingContext context)
        {
            return context.Drawings.Count > 0;
        }


        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SlotNumberAnalysisRanged2CSVCommand");
            this.Filename = $"{context.FilePath}{context.GetGameName()}_RangedSlotNumberAnalysis.csv";

            LoadModel(context);
            SaveToCSV(context);
        }

        private void LoadModel(DrawingContext context)
        {
            for (int slot = 1; slot <= context.SlotCount; slot++)
            {
                for (int number = 1; number <= context.HighestBall; number++)
                {
                    var element = new NumberModel(number, slot, DrawingContext.GameType);
                    element.LoadLastNumberOfDrawingsAndLeave(context.Drawings, TakeDrawwings, LeaveDrawings);
                    numbers.Add(element);
                }

            }

            for (int number = 1; number <= context.HighestBall; number++)
            {
                var element = new NumberModel(number, 0, DrawingContext.GameType);

                foreach (var item in numbers.Where(num => num.BallId == number).ToArray())
                {
                    if (element.DrawingsCount == 0) element.SetDrawingsCount(item.DrawingsCount);
                    element.AddDrawingDates(item.DrawingDates);
                }
                numbers.Add(element);
            }
        }

        private void SaveToCSV(DrawingContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Next Numbers");
            for (int i = 1; i < LeaveDrawings; i++)
            {
                sb.AppendLine(context.Drawings[context.Drawings.Count - i].ToString());
            }
            sb.AppendLine().AppendLine().AppendLine(numbers[0].CSVHeading);
            foreach (var item in numbers)
            {
                sb.AppendLine(item.CSVLine);
            }

            System.IO.File.WriteAllText(Filename, sb.ToString());
        }
    }
}
