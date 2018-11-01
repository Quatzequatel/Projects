using System;

namespace LotteryV2.Domain.Commands
{
    class DefineHistoricalGroups : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("DefineHistoricalGroups");
            //DefineGroupsCommand
            context.DefineGroups();
            context.Drawings.ForEach(i => i.SetContext(context));

            //SetTemplateFingerPrintCommand
            context.Drawings.ForEach(i => i.GetTemplateFingerPrint());

            //DefineHistoricalGroups
            context.DefineHistoricalGroups();
        }
    }
}
