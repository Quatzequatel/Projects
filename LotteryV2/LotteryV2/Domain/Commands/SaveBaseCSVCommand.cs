using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SaveBaseCSVCommand : Command<CommandContext>
    {
        private string _Filename;

        public override bool ShouldExecute(CommandContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}_base.csv";
            return context.Drawings.Count > 500;
        }
        public override void Execute(CommandContext context)
        {
            SaveToCSV(context);
        }

        private void SaveToCSV(CommandContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.Drawings[0].CSVHeading);
            foreach (var item in context.Drawings)
            {
                sb.AppendLine(item.ToCSVString);
            }

            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }
    
    }
}
