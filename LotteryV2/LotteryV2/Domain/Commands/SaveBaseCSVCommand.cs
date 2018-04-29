using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SaveBaseCSVCommand : Command<DrawingContext>
    {
        private string _Filename;

        public override bool ShouldExecute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}_base.csv";
            return context.Drawings.Count > 500;
        }
        public override void Execute(DrawingContext context)
        {
            SaveToCSV(context);
        }


        private void SaveToCSV(DrawingContext context)
        {
            Dictionary<int, SlotGroup> groups = context.GroupsDictionary;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.Drawings[0].CSVHeading + ", Group");
            foreach (var item in context.Drawings)
            {
                sb.Append(item.ToCSVString);
                for (int i = 0; i < context.SlotCount; i++)
                {
                    sb.Append($",{groups[i+1].FindGroupTypes(item.Numbers[i])}");
                }
                sb.AppendLine();
            }

            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }
    
    }
}
