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
}
