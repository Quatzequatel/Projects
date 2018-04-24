using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LotteryV2.Domain.Commands
{
    public class DrawingContext
    {
        public bool IsAlternateContext { get; set; } = false;

        public readonly Game CurrentGame;
        public int SlotCount { get => GetBallCount(); }
        public int HighestBall { get => HighBallNumber(); }
        public readonly DateTime NextDrawingDate;
        private List<Drawing> _Drawings;
        public List<Drawing> Drawings
        {
            get => _Drawings.Where(i => i.DrawingDate >= new DateTime(1995, 1, 1)).ToList();
            private set => _Drawings = value;
        }
        public Dictionary<int, SlotGroup> GroupsDictionary { get; private set; }
        public Dictionary<string, LotoNumber> PickedNumbers { get; private set; }

        public DrawingContext(Game currentGame, DateTime nextDrawing)
        {
            CurrentGame = currentGame;
            NextDrawingDate = nextDrawing;
        }

        public void DefineGroups()
        {
            GroupsDictionary = Groups.DefineGroups(this);
        }

        public void AddToPickedList(LotoNumber number)
        {
            if (PickedNumbers == null) PickedNumbers = new Dictionary<string, LotoNumber>();
            if (Drawings.FirstOrDefault(i => i.KeyString == number.ToString()) == null)
                PickedNumbers[number.ToString()] = number;
        }

        public void AddToPickedList(Dictionary<string, LotoNumber> numbers)
        {
            if (PickedNumbers == null) PickedNumbers = new Dictionary<string, LotoNumber>();
            List<Drawing> cached = Drawings.OrderBy(i => i.KeyString).ToList();
            StringBuilder sb = new StringBuilder();

            foreach (var item in numbers.Where(i => i.Value.Sum >= 121 && i.Value.Sum <= 174))
            {
                //rule 1 goldie-lock sum is between 121 & 174.

                //rule 2 so far no number has been chosen twice.
                if (cached.Where(i => i.KeyString == item.Key).Count() == 0)
                {
                    PickedNumbers[item.Key] = item.Value;
                }
                else
                {
                    sb.AppendLine($"Already a winner, {item.Key}");
                }
            }

            string _Filename = $"{this.FilePath}{this.GetGameName()}_previousWinners.csv";
            System.IO.File.WriteAllText(_Filename, sb.ToString());

        }

        public string GetGameName()
        {
            switch (CurrentGame)
            {
                case Game.Lotto: return "lotto";
                case Game.MegaMillion: return "megamillions";
                case Game.Powerball: return "powerball";
                case Game.Hit5: return "hit5";
            }
            return "";
        }

        public int FirstYear()
        {
            switch (CurrentGame)
            {
                case Game.Lotto:
                    return 1984;
                case Game.MegaMillion:
                    return 2010;
                case Game.Powerball:
                    return 1984;
                case Game.Hit5:
                    return 2007;
                default:
                    throw new IndexOutOfRangeException();
            }
        }

        public int GetBallCount()
        {
            switch (CurrentGame)
            {
                case Game.Lotto: return 6;
                case Game.MegaMillion: return 6;
                case Game.Powerball: return 6;
                case Game.Hit5: return 5;
                default: return -1;
            }
        }

        public int HighBallNumber()
        {
            switch (CurrentGame)
            {
                case Game.Lotto: return 49;
                case Game.MegaMillion: return 75;
                case Game.Powerball: return 79;
                case Game.Hit5: return 39;
                default: return -1;
            }
        }
        public int OptionBallHighNumber()
        {
            switch (CurrentGame)
            {
                case Game.Lotto: return HighBallNumber();
                case Game.MegaMillion: return 25;
                case Game.Powerball: return 26;
                case Game.Hit5: return HighBallNumber();

                default: return -1;
            }
        }

        public List<string> GetLinks(bool updateOnly = false)
        {
            string page = "http://www.walottery.com/WinningNumbers/PastDrawings.aspx";
            string gameParameter = GetGameName();
            List<String> links = new List<string>();
            int startYear = updateOnly && Drawings.Count > 10
                ? Drawings.Last().DrawingDate.Year
                : FirstYear();
            for (int i = startYear; i <= DateTime.Now.Year; i++)
            {
                links.Add($"{page}?gamename={gameParameter}&unittype=year&unitcount={i}");
            }

            return links;
        }

        public Tuple<int, int> GetMinMaxForSlot(int slot)
        {
            switch (CurrentGame)
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

        public string FilePath => "C:\\Users\\Steven\\Documents\\";

        public void SetDrawings(List<Drawing> drawings)
        {
            if (Drawings != null) throw new AccessViolationException("Drawings has already been set.");
            Drawings = drawings;
        }

        public void ReplaceDrawings(List<Drawing> drawings)
        {
            Drawings = drawings;
        }

    }
}
