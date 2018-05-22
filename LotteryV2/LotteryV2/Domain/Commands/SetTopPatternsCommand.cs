namespace LotteryV2.Domain.Commands
{

    public class SetTopPatternsCommand : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            FindTopPatterns(context);
        }

        private void FindTopPatterns(DrawingContext context)
        {
            //    foreach (var item in context.Drawings.)
            //    {

            //    }
        }
    }
}
