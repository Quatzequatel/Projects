using Newtonsoft.Json;
using System.Collections.Generic;

namespace LotteryV2.Domain.Commands
{

    internal class LoadFromFile : Command<DrawingContext>
    {
        private string _Filename;

        public override bool ShouldExecute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}.json";
            return System.IO.File.Exists(_Filename);
        }
        public override void Execute(DrawingContext context)
        {
            LoadDrawingsFromFile(context);
        }

        void LoadDrawingsFromFile(DrawingContext context)
        {
            List<Drawing> data = JsonConvert.DeserializeObject<List<Drawing>>(System.IO.File.ReadAllText(_Filename));
            context.SetDrawings(data);
        }
    }
}
