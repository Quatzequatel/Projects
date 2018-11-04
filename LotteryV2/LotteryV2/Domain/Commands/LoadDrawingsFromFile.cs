using Newtonsoft.Json;
using System;
using LotteryV2.Domain.Model;
using System.Collections.Generic;

namespace LotteryV2.Domain.Commands
{

    internal class LoadDrawingsFromFile : Command<DrawingContext>
    {
        private string filename;

        public override bool ShouldExecute(DrawingContext context)
        {
            filename = $"{context.FilePath}{context.GetGameName()}.json";
            return System.IO.File.Exists(filename);
        }
        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("LoadDrawingsFromFile");
            LoadDrawingsFromFileExecute(context);
        }

        void LoadDrawingsFromFileExecute(DrawingContext context)
        {
            List<Drawing> data = JsonConvert.DeserializeObject<List<Drawing>>(System.IO.File.ReadAllText(filename));
            context.SetDrawings(data);
        }
    }
}
