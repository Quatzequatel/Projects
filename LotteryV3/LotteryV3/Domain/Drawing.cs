using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LotteryV3.Domain
{
    public class Drawing
    {
        private int[] balls = new int[6];
        private string drawingDate = string.Empty;

        [JsonIgnore]
        public DrawingContext Context { get; private set; }

        public GameType Game { get; set; }

        public int[] Numbers { get => balls; set => balls = value; }
        public int Sum => balls.Sum();
        public DateTime DrawingDate { get; private set; }
        public Drawing SetDrawingDate(string date) { DrawingDate = DateTime.Parse(date); return this; }
        public Decimal PrizeAmount { get; set; }
        public Drawing SetPrizeAmount(decimal amount) { PrizeAmount = amount; return this; }
        public int Winners { get; set; }
        public Drawing SetWinners(int winners) { Winners = winners; return this; }
    }

    public class DrawingContext
    {
        private GameType _game;
        public GameType Game => _game;

        public int SlotCount { get => GetBallCount(); }
        public int HighestBall { get => HighBallNumber(); }
        public readonly DateTime NextDrawingDate;
        public List<Drawing> Drawings { get; private set; }

        public DrawingContext(GameType currentGame, DateTime nextDrawing)
        {
            _game = currentGame;
            NextDrawingDate = nextDrawing;
        }

        public string GetGameName()
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

        public List<NumberInfo> GetNumberInfoList()
        {
            List<NumberInfo> result = new List<NumberInfo>();

            for (int slot = 1; slot <= SlotCount; slot++)
            {
                for (int index = 1; index <= HighestBall; index++)
                {
                    var number = new NumberInfo(index, slot, Game);
                    number.LoadDrawings(Drawings);
                    result.Add(number);
                }
            }

            //load number model for all slots.
            for (int number = 1; number <= HighestBall; number++)
            {
                var element = new NumberInfo(number, 0, Game);

                foreach (var item in result.Where(num => num.Id == number).ToArray())
                {
                    if (element.DrawingsCount == 0)
                    {
                        element.SetDrawingsCount(item.DrawingsCount);
                    }
                    element.AddDrawingDates(item.DrawingDates);
                }
                result.Add(element);
            }

            return result;
        }
    }

    /// <summary>
    /// PropabilityGroup contains
    /// </summary>
    public class PropabilityGroup
    {
        private int _slotId;
        private GameType _game;
        private PropabilityType _Propability;
        private List<NumberInfo> _NumberSet;

        public PropabilityGroup(int slotId, GameType game, PropabilityType propability)
        {
            _slotId = slotId;
            _game = game;
            _Propability = propability;
        }

        public void AddNumbers(IEnumerable<NumberInfo> values)
        {
            IEnumerable<NumberInfo> slotValuesList = values.Where(i => i.SlotId == _slotId).ToArray();
            int fullGroupSize = slotValuesList.Count();
            if (fullGroupSize == 0) return;

            int zeroGroupSize = slotValuesList.Where(i => i.PercentChosen == 0).Count();
            int groupSizeSum = zeroGroupSize == 0 ? fullGroupSize : fullGroupSize - zeroGroupSize;

            int subGroupSize = (int)Math.Floor(groupSizeSum / 5.0);

            switch (_Propability)
            {
                case PropabilityType.Zero:
                    _NumberSet = new List<NumberInfo>(values.Where(i => i.SlotId == _slotId && i.PercentChosen <= 0.1).ToArray());
                    break;
                case PropabilityType.Low:
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 4).Take(subGroupSize));
                    break;
                case PropabilityType.MidLow:
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 3).Take(subGroupSize));
                    break;
                case PropabilityType.Mid:
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize * 2).Take(subGroupSize));
                    break;
                case PropabilityType.MidHigh:
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Skip(subGroupSize).Take(subGroupSize));
                    break;
                case PropabilityType.High:
                    _NumberSet = new List<NumberInfo>(slotValuesList.OrderByDescending(i => i.PercentChosen).Take(subGroupSize));
                    break;
                default:
                    break;
            }

        }
    }

    public class PropabilityGroups
    {
        private int _slotId;
        private GameType _game;
        private DrawingContext _Context;
        private Dictionary<PropabilityType, PropabilityGroup> _Groups = new Dictionary<PropabilityType, PropabilityGroup>();

        public PropabilityGroups(DrawingContext context, int slotId)
        {
            _slotId = slotId;
            _game = context.Game;
            foreach (PropabilityType group in (PropabilityType[])Enum.GetValues(typeof(PropabilityType)))
            {
                _Groups[group] = new PropabilityGroup(slotId, _game, group);
                _Groups[group].AddNumbers(context.GetNumberInfoList());
            }
        }
    }

    /// <summary>
    /// Number stores basic information about a ball; such as all dates drawn for given slot
    /// </summary>
    public class Number
    {
        private int _Id;
        private int _slotId;
        private GameType _game;

        private readonly List<DateTime> drawingDates = new List<DateTime>();
        public List<DateTime> DrawingDates { get => drawingDates; }

        public int Id { get => _Id; }
        public int SlotId { get => _slotId; }
        public GameType Game { get => _game; }

        public void AddDrawingDate(DateTime date) => drawingDates.Add(date);
        public void AddDrawingDates(IEnumerable<DateTime> dates) => drawingDates.AddRange(dates);
        public Number(int Id, int slotId, GameType game)
        {
            _Id = Id;
            _slotId = slotId;
            _game = game;
        }
    }

    /// <summary>
    /// NumberInfo builds on number storing more analysis data on a given slot:ball pair and can hold data in 
    /// relationship to other numbers for given slot.
    /// </summary>
    public class NumberInfo : Number
    {
        public int TotalSum;
        public double AvgSum;
        public int MinSum;
        public int MaxSum;

        public double SumSTD;
        public double SumVariance;

        public PropabilityGroup Group;

        public int TimesChosen => DrawingDates.Count;
        public int DrawingsCount { get; private set; }
        public void SetDrawingsCount(int value) => DrawingsCount = value;

        public double PercentChosen => ((double)TimesChosen / DrawingsCount) * 100;

        public NumberInfo(int Id, int slotId, GameType game) : base(Id, slotId, game)
        {

        }

        public void LoadDrawings(List<Drawing> drawings)
        {
            var list = drawings.Where(drawing => drawing.Game == Game && drawing.Numbers[SlotId - 1] == Id).ToArray();
            foreach (var item in list)
            {
                base.AddDrawingDate(item.DrawingDate);
                TotalSum = +item.Sum;
            }
            DrawingsCount = drawings.Count;
            if (list.Count() == 0) return;
            MinSum = list.Select(i => i.Sum).Min();
            MaxSum = list.Select(i => i.Sum).Max();
            AvgSum = list.Select(i => i.Sum).Average();
            SumSTD = list.Select(i => i.Sum).ToList().StandardDeviation();
            SumVariance = list.Select(i => i.Sum).ToList().Variance();
        }



        public string CSVHeading => new string[] { "Game", "Slot", "Number", "Times Chosen", "Chosen %", "AvgSum", "MinSum", "MaxSum", "SumSTD", "SumVariance" }.CSV();
        public string CSVLine => new string[]
        {
            $"{Game}",
            $"{SlotId}",
            $"{Id}",
            $"{TimesChosen}",
            $"{PercentChosen}",
            $"{AvgSum}",
            $"{MinSum}",
            $"{MaxSum}",
            $"{SumSTD}",
            $"{SumVariance}"
        }.CSV();
    }

    public class Pattern
    {

    }

    public enum PropabilityType
    {
        Zero,
        Low,
        MidLow,
        Mid,
        MidHigh,
        High
    }
    public enum GameType
    {
        Lotto,
        MegaMillion,
        Powerball,
        Hit5,
        Match4
    }
}
