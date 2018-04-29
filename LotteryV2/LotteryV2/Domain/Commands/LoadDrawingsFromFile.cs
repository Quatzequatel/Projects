using Newtonsoft.Json;
using System.Collections.Generic;

namespace LotteryV2.Domain.Commands
{

    internal class LoadDrawingsFromFile : Command<DrawingContext>
    {
        private string _Filename;

        public override bool ShouldExecute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}.json";
            return System.IO.File.Exists(_Filename);
        }
        public override void Execute(DrawingContext context)
        {
            LoadDrawingsFromFileExecute(context);
        }

        void LoadDrawingsFromFileExecute(DrawingContext context)
        {
            List<Drawing> data = JsonConvert.DeserializeObject<List<Drawing>>(System.IO.File.ReadAllText(_Filename));
            context.SetDrawings(data);
        }
    }

    internal class ModifyDrawingContext : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }
        public override void Execute(DrawingContext context)
        {
            
        }

        public void TBD (DrawingContext context)
        {
            context.SetNextDrawingDate(new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day));
        }
    }
}
