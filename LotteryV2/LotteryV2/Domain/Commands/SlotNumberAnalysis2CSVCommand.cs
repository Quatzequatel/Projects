using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SlotNumberAnalysis2CSVCommand : Command<CommandContext>
    {
        private string _Filename;
        private List<SlotNumberAnalysis> numbers = new List<SlotNumberAnalysis>();

        public override void Execute(CommandContext context)
        {
            _Filename = _Filename = $"{context.FilePath}{context.GetGameName()}_SlotNumberAnalysis.csv";
            LoadModel(context);
            SaveToCSV(context);
        }

        private void LoadModel(CommandContext context)
        {
            for (int slot = 1; slot <= context.SlotCount; slot++)
            {
                for (int number = 1; number <= context.HighestBall; number++)
                {
                    var element = new SlotNumberAnalysis(number, slot, context.CurrentGame);
                    element.LoadDrawings(context.Drawings);
                    numbers.Add(element);
                }
            }

            for (int number = 1; number <= context.HighestBall; number++)
            {
                var element = new SlotNumberAnalysis(number, 0, context.CurrentGame);

                foreach (var item in numbers.Where(num => num.Id == number).ToArray())
                {
                    if (element.DrawingsCount == 0) element.SetDrawingsCount(item.DrawingsCount);
                    element.AddDrawingDates(item.DrawingDates);
                }
                numbers.Add(element);
            }
        }

        private void SaveToCSV(CommandContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(numbers[0].CSVHeading);
            foreach (var item in numbers)
            {
                sb.AppendLine(item.CSVLine);
            }

            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }
    }
}
