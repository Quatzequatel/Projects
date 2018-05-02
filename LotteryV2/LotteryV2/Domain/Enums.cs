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
        Hit5,
        Match4
    }


    public enum SubSets
    {
        Zero = 1,
        Low = 2,
        MidLow = 4,
        Mid = 8,
        MidHigh = 16,
        High = 32
    }

    public enum TemplateSets
    {
        Black = 0,
        Aqua = 2,
        Sunrise = 7,
        RedHot = 100
    }
}
