using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lottery
{
    public static class Utilities
    {

        public static string GetGameName()
        {
            switch (Scraper.CurrentGame)
            {
                case Game.Lotto: return "lotto";
                case Game.MegaMillion: return "megamillions";
                case Game.Powerball: return "powerball";
                case Game.Hit5: return "hit5";
            }
            return "";
        }

        public static int GetBallCount()
        {
            switch (Scraper.CurrentGame)
            {
                case Game.Lotto: return 6;
                case Game.MegaMillion: return 6;
                case Game.Powerball: return 6;
                case Game.Hit5: return 5;
                default: return -1;
            }
        }

        public static int HighBallNumber()
        {
            switch (Scraper.CurrentGame)
            {
                case Game.Lotto: return 49;
                case Game.MegaMillion: return 75;
                case Game.Powerball: return 79;
                case Game.Hit5: return 39;
                default: return -1;
            }
        }
        public static int OptionBallHighnumber()
        {
            switch (Scraper.CurrentGame)
            {
                case Game.Lotto: return HighBallNumber();
                case Game.MegaMillion: return 25;
                case Game.Powerball: return 26;
                case Game.Hit5: return HighBallNumber();

                default: return -1;
            }
        }

        public static Tuple<int, int> GetMinMaxForSlot(int slot)
        {
            switch (Scraper.CurrentGame)
            {
                case Game.Lotto:
                    switch (slot)
                    {
                        case 0: return new Tuple<int, int>(1, 29);
                        case 1: return new Tuple<int, int>(2, 39);
                        case 2: return new Tuple<int, int>(3, 45);
                        case 3: return new Tuple<int, int>(7, 47);
                        case 4: return new Tuple<int, int>(12, 48);
                        case 5: return new Tuple<int, int>(18, 49);
                        default: return new Tuple<int, int>(0, 0);
                    }
                case Game.MegaMillion:
                    switch (slot)
                    {
                        case 0: return new Tuple<int, int>(1, 41);
                        case 1: return new Tuple<int, int>(2, 58);
                        case 2: return new Tuple<int, int>(4, 68);
                        case 3: return new Tuple<int, int>(11, 70);
                        case 4: return new Tuple<int, int>(21, 70);
                        case 5: return new Tuple<int, int>(1, 25);
                        default: return new Tuple<int, int>(0, 0);
                    }
                case Game.Powerball:
                    switch (slot)
                    {
                        case 0: return new Tuple<int, int>(1, 29);
                        case 1: return new Tuple<int, int>(2, 39);
                        case 2: return new Tuple<int, int>(3, 45);
                        case 3: return new Tuple<int, int>(7, 47);
                        case 4: return new Tuple<int, int>(12, 48);
                        case 5: return new Tuple<int, int>(1, 26);
                        default: return new Tuple<int, int>(0, 0);
                    }
                case Game.Hit5:
                    switch (slot)
                    {
                        case 0: return new Tuple<int, int>(1, 29);
                        case 1: return new Tuple<int, int>(2, 39);
                        case 2: return new Tuple<int, int>(3, 39);
                        case 3: return new Tuple<int, int>(7, 39);
                        case 4: return new Tuple<int, int>(12, 39);
                        default: return new Tuple<int, int>(0, 0);
                    }
                default:
                    return new Tuple<int, int>(0, 0);
            }
        }

        public static string Path => "C:\\Users\\Steven\\Documents\\";
    }
}
