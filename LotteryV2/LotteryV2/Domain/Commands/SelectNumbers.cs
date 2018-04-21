using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain.Commands
{
    public class SelectNumbers: Command<DrawingContext>
    {
        DrawingContext drawingContext;
        public override void Execute(DrawingContext context)
        {
            drawingContext = context;
        }
    }
}
