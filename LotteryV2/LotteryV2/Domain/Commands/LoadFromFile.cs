using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{

    internal class LoadFromFile : Command<CommandContext>
    {
        private string _Filename;

        public override bool ShouldExecute(CommandContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}.json";
            return System.IO.File.Exists(_Filename);
        }
        public override void Execute(CommandContext context)
        {
            LoadDrawingsFromFile(context);
        }

        void LoadDrawingsFromFile(CommandContext context)
        {
            context.SetDrawings(JsonConvert.DeserializeObject<List<Drawing>>(System.IO.File.ReadAllText(_Filename)));
        }
    }
}
