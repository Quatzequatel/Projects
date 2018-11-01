using System.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace LotteryV2.Domain.Commands
{
    public class DrawingContext
    {
        public bool IsAlternateContext { get; set; } = false;

        /// <summary>
        /// type of game in current context
        /// </summary>
        public static Game GameType;
        /// <summary>
        /// how many slots in current game.
        /// </summary>
        public int SlotCount { get => GetBallCount(); }
        /// <summary>
        /// number of the highest ball.
        /// </summary>
        public int HighestBall { get => HighBallNumber(); }

        public int SampleSize { get; set; }

        public Boolean SkipDownload { get; set; }

        public Boolean ShouldExecuteSetHistoricalPeriods { get; set; }

        public Dictionary<HistoricalPeriods, List<HistoricalPeriodItem>> Periods { get; set; }

        /// <summary>
        /// Start date of analysis period.
        /// </summary>
        public DateTime StartDate { get; private set; }
        /// <summary>
        /// Date of drawing or end of analysis period.
        /// </summary>
        public DateTime EndDate { get; private set; }
        public void SetPeriodContext(HistoricalPeriods period)
        {
            if (period != HistoricalPeriods.All)
            {
                SetDrawingsDateRange(EndDate.AddDays(-(int)period), EndDate);
            }
            else
            {
                SetDrawingsDateRange(AllDrawings.OrderByDescending(i => i.DrawingDate).Last().DrawingDate, EndDate);
            }
        }

        public void SetDrawingsDateRange(DateTime startDate, DateTime endDate)
        {
            bool refreshGroups = (startDate != StartDate || endDate != EndDate);
            StartDate = startDate;
            EndDate = endDate;
            if (refreshGroups && this.Drawings?.Count > 0)
            {
                this.DefineGroups();
                this.Drawings.ForEach(i => i.SetContext(this));
                this.Drawings.ForEach(i => i.GetTemplateFingerPrint());
            }
        }

        public void SetHistoricalPeriods()
        {
            Console.WriteLine("SetHistoricalPeriods");
            DateTime firstDrawingDate = AllDrawings.First().DrawingDate;
            //Find the first possible day to have periods applied.
            foreach (var drawing in AllDrawings.Where(i => i.DrawingDate > firstDrawingDate.AddDays((int)HistoricalPeriods.Wk2)))
            {
                foreach (var period in (HistoricalPeriods[])Enum.GetValues(typeof(HistoricalPeriods)))
                {
                    DateTime currentPeriod = drawing.DrawingDate.AddDays(-(int)period);
                    if (currentPeriod >= firstDrawingDate)
                    {
                        if (period == HistoricalPeriods.All)
                        {
                            this.SetDrawingsDateRange(firstDrawingDate, drawing.DrawingDate.AddDays(-1));
                        }
                        else
                        {
                            this.SetDrawingsDateRange(
                                drawing.DrawingDate.Date.AddDays(-(int)period),
                                drawing.DrawingDate.Date.AddDays(-1));
                        }
                        drawing.SetContext(this);
                        drawing.SetHistoricalPeriodFingerPrint(period, drawing.GetTemplateFingerPrint());
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        public DateTime NextDrawingDate { get; private set; }
        public DrawingContext SetNextDrawingDate(DateTime value) { NextDrawingDate = value; return this; }

        public List<Drawing> Drawings
        {
            get
            {
                return AllDrawings?.Where(i => i.DrawingDate >= StartDate && i.DrawingDate <= EndDate).ToList();
            }
        }

        public List<Drawing> AllDrawings { get; private set; }

        public List<NumberModel> NumberModelList { get; private set; }

        public Dictionary<int, SlotGroup> GroupsDictionary { get; private set; }
        public Dictionary<string, LotoNumber> PickedNumbers { get; private set; }

        public DateTime? LastDrawingDate { get { return Drawings.Count > 0 ? Drawings?.Last<Drawing>()?.DrawingDate: null; } }
        public DateTime? FirstDrawingDate { get { return Drawings.Count > 0 ? Drawings?.First<Drawing>()?.DrawingDate: null; } }

        public DrawingContext(Game currentGame, DateTime? nextDrawing = null)
        {
            GameType = currentGame;
            NextDrawingDate = nextDrawing != null ? nextDrawing.Value : new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day);
            this.SetSlotCount();
        }

        /// <summary>
        /// Creates a dictionary of of Slot groups based on the Context Drawings values.
        /// To do this requires LoadSlotModel() to execute which returns a list of NumberModels 
        /// from the Context Drawings. Then DefineGroups() consumes the list of NumberModesl to
        /// define the slot groups. Which are a probabiltiy group from the Drawings.
        /// </summary>
        public void DefineGroups()
        {
            NumberModelList = Groups.LoadSlotModel(this);
            GroupsDictionary = Groups.DefineGroups(SlotCount, GameType, NumberModelList);
        }

        public HistoricalGroups HistoricalGroups { get; set; } = new HistoricalGroups();

        public void DefineHistoricalGroups()
        {
            foreach (var period in (HistoricalPeriods[])Enum.GetValues(typeof(HistoricalPeriods)))
            {
                //Set the date range for Drawings.
                SetPeriodContext(period);
                //Extract the Group Definitions.
                DefineGroups();
                //TODO: Use this context to organize historical period information per number.
                foreach (var slotgroup in GroupsDictionary)
                {
                    HistoricalGroups.PeriodGroups[period][slotgroup.Key] = slotgroup.Value;
                }
                 
            }
        }

        public void AddToPickedList(LotoNumber number)
        {
            if (PickedNumbers == null) PickedNumbers = new Dictionary<string, LotoNumber>();
            if (Drawings.FirstOrDefault(i => i.KeyString == number.ToString()) == null)
                PickedNumbers[number.ToString()] = number;
        }

        public void GetNextDrawingNumber(Drawing nextDrawing)
        {

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
            switch (GameType)
            {
                case Game.Lotto: return "lotto";
                case Game.MegaMillion: return "megamillions";
                case Game.Powerball: return "powerball";
                case Game.Hit5: return "hit5";
                case Game.Match4: return "match4";
            }
            return "";
        }

        public int FirstYear()
        {
            switch (GameType)
            {
                case Game.Lotto:
                    return 1984;
                case Game.Match4:
                    return 2008;
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

        public static int GetBallCount()
        {
            switch (GameType)
            {
                case Game.Lotto: return 6;
                case Game.MegaMillion: return 6;
                case Game.Powerball: return 6;
                case Game.Hit5: return 5;
                case Game.Match4: return 4;
                default: return -1;
            }
        }

        public int HighBallNumber()
        {
            switch (GameType)
            {
                case Game.Lotto: return 49;
                case Game.MegaMillion: return 70;
                case Game.Powerball: return 79;
                case Game.Hit5: return 39;
                case Game.Match4: return 24;
                default: return -1;
            }
        }
        public int OptionBallHighNumber()
        {
            switch (GameType)
            {
                case Game.Lotto: return HighBallNumber();
                case Game.MegaMillion: return 25;
                case Game.Powerball: return 26;
                case Game.Hit5: return HighBallNumber();

                default: return -1;
            }
        }

        public Boolean HasOptionalBall()
        {
            switch(GameType)
            {
                case Game.MegaMillion:
                case Game.Powerball:
                    return true;
                default:
                    return false;
            }
        }

        public List<string> GetLinks(bool isUpdate = false)
        {
            string page = "http://www.walottery.com/WinningNumbers/PastDrawings.aspx";
            string gameParameter = GetGameName();
            List<String> links = new List<string>();
            int startYear = isUpdate && Drawings.Count > 10
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
            switch (GameType)
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

        public string FileHistoricalPeriods => $"{this.FilePath}{this.GetGameName()}-HistoricalPeriods.json";

        public string FileHistoricalSummary => $"{this.FilePath}{this.GetGameName()}-HistoricalSummary.csv";

        public string FilePath => "C:\\Users\\Steven\\Documents\\";

        public void SetDrawings(List<Drawing> drawings)
        {
            if (Drawings != null) throw new AccessViolationException("Drawings has already been set.");
            AllDrawings = drawings.OrderBy(i=> i.DrawingDate).ToList();
        }

        public void ReplaceDrawings(List<Drawing> drawings)
        {
            AllDrawings = drawings.OrderBy(i => i.DrawingDate).ToList();
        }

        public List<string> PickNumbers(HistoricalPeriods period, int howMany)
        {
            List<string> result = new List<string>();



            return result;
        }

    }
}
