using Newtonsoft.Json;
using System;
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

    internal class DefineDrawingDateRangeCommand : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }
        public override void Execute(DrawingContext context)
        {
            Console.WriteLine($"Begin type {context.GetGameName()} DateRange: {context.StartDate} to {context.EndDate}");
            DefineDrawingDateRange(context);
        }

        public void DefineDrawingDateRange (DrawingContext context)
        {
            DateTime StartDate = System.DateTime.Now.AddMonths(-60);
            DateTime EndDate = new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);

            context.SetDrawingsDateRange(StartDate, EndDate);

            Console.WriteLine($"Begin type {context.GetGameName()} DateRange: {context.StartDate} to {context.EndDate}");

        }
    }
}
