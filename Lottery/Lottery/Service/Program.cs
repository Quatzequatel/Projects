using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    class Program
    {
        static void Main(string[] args)
        {
            Scraper.CurrentGame = Game.MegaMillion;
            Scraper.LoadData(new DateTime(2018, 4, 1), 2000, 2018);
        }


            
    }
}
