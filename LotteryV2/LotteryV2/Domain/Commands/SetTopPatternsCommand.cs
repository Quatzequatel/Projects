using System;

namespace LotteryV2.Domain.Commands
{

    public class SetTopPatternsCommand : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SetTopPatternsCommand");
            FindTopPatterns(context);
        }

        /*
         Todo: 
            1. find top patterns for each period.
            2. Select numbers for each period by group.
            3. Distill numbers to final selections.
         */

        private void FindTopPatterns(DrawingContext context)
        {
            //    foreach (var item in context.Drawings.)
            //    {

            //    }
        }
    }
}
