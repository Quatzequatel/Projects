namespace LotteryV2.Domain.Commands
{
    class SetTemplateFingerPrintCommand : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            context.Drawings.ForEach(i => i.GetTemplateFingerPrint());
        }
    }
}
