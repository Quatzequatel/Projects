using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    internal class CreatNumbersModelCommand : Command<DrawingContext>
    {

        public override void Execute(DrawingContext context, IEnumerable<string> additionalMetaData)
        {
            Execute(context);
        }
        public override void Execute(DrawingContext context)
        {
            throw new NotImplementedException();
        }

    }

    public class AnalysisContext
    {
        public AnalysisContext(DrawingContext drawingContext)
        {
            NextDrawingDate = drawingContext.NextDrawingDate;
            DrawingContext = drawingContext;
        }
        public readonly DateTime NextDrawingDate;
        public readonly DrawingContext DrawingContext;
        
    }

    public class NumberModelCommandFactory : ICommandFactory<AnalysisContext>
    {
        public LinkedList<Command<AnalysisContext>> CreateAlternateCommands(AnalysisContext context)
        {
            throw new NotImplementedException();
        }

        public LinkedList<Command<AnalysisContext>> CreateCommands(AnalysisContext context)
        {
            throw new NotImplementedException();
        }
    }
}
