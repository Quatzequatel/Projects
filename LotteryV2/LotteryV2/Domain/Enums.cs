using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain
{
    public enum Game
    {
        Lotto,
        MegaMillion,
        Powerball,
        Hit5
    }


    public enum GroupType
    {
        Zero,
        Low,
        MidLow,
        Mid,
        MidHigh,
        High
    }
}
