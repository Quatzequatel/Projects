﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SaveJsonToFileCommand : Command<CommandContext>
    {
        private string _Filename;


        public override bool ShouldExecute(CommandContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}.json";
            return context.Drawings.Count() > 10;
        }

        public override void Execute(CommandContext context)
        {
            SaveToJSON(context);
        }

        private void SaveToJSON(CommandContext context)
        {
            System.IO.File.WriteAllText(_Filename, JsonConvert.SerializeObject(context.Drawings.OrderBy(b => b.DrawingDate), Formatting.Indented));
        }
    }
}
