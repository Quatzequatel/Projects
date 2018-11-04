using System;

namespace LotteryV2.Domain.Commands
{
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
}
