using System;
using System.Collections.Generic;
using System.Linq;

namespace LotteryV3.Domain.Entities
{
    public class DrawingContext
    {
        private GameType _game;
        public GameType Game => _game;

        public int SlotCount { get => GetBallCount(); }
        public int HighestBall { get => HighBallNumber(); }
        public readonly DateTime NextDrawingDate;
        public readonly DateTime FirstDrawingDate;
        public List<Drawing> Drawings { get; private set; }
        public Dictionary<string, TrendValue> TrendDictionary { get; set; } = new Dictionary<string, TrendValue>();

        public string FilePath => "C:\\Users\\Steven\\Documents\\";

        private List<NumberInfo> NumberInfoListCache { get; set; }

        private PropabilityGroups PropabilityGroups;
        public PropabilityGroups GetPropabilityGroups => PropabilityGroups;

        public DrawingContext(GameType currentGame, DateTime nextDrawing, DateTime firstDrawingDate)
        {
            _game = currentGame;
            NextDrawingDate = nextDrawing;
            FirstDrawingDate = firstDrawingDate;
        }

        public void DefineGroups()
        {
            PropabilityGroups = new PropabilityGroups(this);
        }
        public int FirstYear()
        {
            switch (_game)
            {
                case GameType.Lotto:
                    return 1984;
                case GameType.MegaMillion:
                    return 2010;
                case GameType.Powerball:
                    return 1984;
                case GameType.Hit5:
                    return 2007;
                case GameType.Match4:
                    return 2008;
                default:
                    throw new IndexOutOfRangeException();
            }
        }
        public int GetBallCount()
        {
            switch (_game)
            {
                case GameType.Lotto: return 6;
                case GameType.MegaMillion: return 6;
                case GameType.Powerball: return 6;
                case GameType.Hit5: return 5;
                case GameType.Match4: return 4;
                default: return -1;
            }
        }
        public string GetGameName => GameName();
        private string GameName()
        {
            switch (_game)
            {
                case GameType.Lotto: return "lotto";
                case GameType.MegaMillion: return "megamillions";
                case GameType.Powerball: return "powerball";
                case GameType.Hit5: return "hit5";
                case GameType.Match4: return "match4";
            }
            return "";
        }


        public List<string> GetLinks(bool updateOnly = false)
        {
            string page = "http://www.walottery.com/WinningNumbers/PastDrawings.aspx";
            string gameParameter = GetGameName;
            List<String> links = new List<string>();
            int startYear = updateOnly && Drawings.Count > 10
                ? Drawings.OrderBy(i => i.DrawingDate).Last().DrawingDate.Year
                : FirstYear();
            for (int i = startYear; i <= DateTime.Now.Year; i++)
            {
                links.Add($"{page}?gamename={gameParameter}&unittype=year&unitcount={i}");
            }

            return links;
        }

        public List<NumberInfo> GetNumberInfoList()
        {
            if (NumberInfoListCache == null)
            {
                NumberInfoListCache = new List<NumberInfo>();
            }
            else
            {
                return NumberInfoListCache;
            }

            for (int slot = 1; slot <= SlotCount; slot++)
            {
                for (int index = 1; index <= HighestBall; index++)
                {
                    var number = new NumberInfo(index, slot, Game, FirstDrawingDate);
                    number.LoadDrawings(Drawings);
                    NumberInfoListCache.Add(number);
                }
            }

            //load number model for all slots.
            for (int number = 1; number <= HighestBall; number++)
            {
                var element = new NumberInfo(number, 0, Game, FirstDrawingDate);

                foreach (var item in NumberInfoListCache.Where(num => num.Id == number).ToArray())
                {
                    if (element.DrawingsCount == 0)
                    {
                        element.SetDrawingsCount(item.DrawingsCount);
                    }
                    element.MergeDrawingDatesThenResetDrawingIntervals(0, number, item.DrawingDates);
                }
                NumberInfoListCache.Add(element);
            }

            return NumberInfoListCache;
        }

        public int HighBallNumber()
        {
            switch (_game)
            {
                case GameType.Lotto: return 49;
                case GameType.MegaMillion: return 75;
                case GameType.Powerball: return 79;
                case GameType.Hit5: return 39;
                case GameType.Match4: return 24;
                default: return -1;
            }
        }
        public int OptionBallHighNumber()
        {
            switch (_game)
            {
                case GameType.Lotto: return HighBallNumber();
                case GameType.MegaMillion: return 25;
                case GameType.Powerball: return 26;
                case GameType.Hit5: return HighBallNumber();
                case GameType.Match4: return HighBallNumber();

                default: return -1;
            }
        }

        public void ReplaceDrawings(List<Drawing> drawings)
        {
            Drawings = drawings;
            SetContextOnDrawings();
        }

        public void SetContextOnDrawings()
        {
            Drawings.ForEach(drawing => drawing.SetContext(this));
            for (int slotId = 0; slotId < this.GetBallCount(); slotId++)
            {
                for (int id = 1; id < this.HighestBall; id++)
                {
                    var list = Drawings.Where(drawing => drawing.Game == Game && drawing.Numbers[slotId] == id).ToArray();
                    for (int drawing = 0; drawing < list.Length; drawing++)
                    {
                        TrendDictionary[new TrendKey(slotId, list[drawing].DrawingDate).ToString()] = new TrendValue(slotId, list[drawing].Numbers[slotId],
                            list[drawing].DrawingDate, drawing > 0 ? list[drawing - 1].DrawingDate : this.FirstDrawingDate, this.FirstDrawingDate);
                    }
                }
            }
        }




        public void SetDrawings(List<Drawing> drawings)
        {
            if (Drawings != null) throw new AccessViolationException("Drawings has already been set.");
            Drawings = drawings;
            SetContextOnDrawings();
        }
    }
}
