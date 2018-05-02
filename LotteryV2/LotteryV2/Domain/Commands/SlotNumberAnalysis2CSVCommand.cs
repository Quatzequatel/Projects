using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SlotNumberAnalysis2CSVCommand : Command<DrawingContext>
    {
        private string _Filename;
        private List<NumberModel> numbers = new List<NumberModel>();
        Dictionary<int, SlotGroup> groups;

        public override void Execute(DrawingContext context)
        {
            _Filename = _Filename = $"{context.FilePath}{context.GetGameName()}_SlotNumberAnalysis.csv";
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
                sb.AppendLine(item.CSVLine + $",{groups[item.SlotId].FindGroupType(item.Id)}");
            }

            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }
    }
}
