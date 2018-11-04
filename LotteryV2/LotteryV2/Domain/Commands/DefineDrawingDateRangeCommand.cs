using System;

namespace LotteryV2.Domain.Commands
{
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
