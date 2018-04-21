using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SlotNumberAnalysisRanged2CSVCommand : Command<DrawingContext>
    {
        private string _Filename;
        private List<NumberModel> numbers = new List<NumberModel>();
        private int _TakeDrawwings = 1000;
        private int _LeaveDrawings = 10;
        private List<Tuple<int, double>> dataBag = new List<Tuple<int, double>>();

        public override void Execute(DrawingContext context)
        {
            _Filename = _Filename = $"{context.FilePath}{context.GetGameName()}_RangedSlotNumberAnalysis.csv";
            LoadModel(context);
            SaveToCSV(context);
        }

        private void LoadModel(DrawingContext context)
        {
            for (int slot = 1; slot <= context.SlotCount; slot++)
            {
                for (int number = 1; number <= context.HighestBall; number++)
                {
                    var element = new NumberModel(number, slot, context.CurrentGame);
                    element.LoadLastNumberOfDrawingsAndLeave(context.Drawings, _TakeDrawwings, _LeaveDrawings);
                    numbers.Add(element);
                }

            }

            for (int number = 1; number <= context.HighestBall; number++)
            {
                var element = new NumberModel(number, 0, context.CurrentGame);

                foreach (var item in numbers.Where(num => num.Id == number).ToArray())
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
            for (int i = 1; i < _LeaveDrawings; i++)
            {
                sb.AppendLine(context.Drawings[context.Drawings.Count - i].ToString());
            }
            sb.AppendLine().AppendLine().AppendLine(numbers[0].CSVHeading);
            foreach (var item in numbers)
            {
                sb.AppendLine(item.CSVLine);
            }

            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }
    }
}
