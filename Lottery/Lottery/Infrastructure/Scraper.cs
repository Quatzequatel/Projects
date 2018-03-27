using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Lottery;

namespace Lottery
{
    public class Scraper
    {
        public static Game CurrentGame { get; set; }
        public static DateTime NextDrawingDate { get => new DateTime(2018, 3, 23); }

        public static void LoadData(DateTime nextDrawing, int startYear, int endYear)
        {
            
            List<string> links = GetLinks(startYear, endYear);
            var drawings = SaveLoad(nextDrawing, links);
            var slots = new Slots(Utilities.GetBallCount(), Utilities.HighBallNumber());
            slots.AddDrawing(drawings);

            SaveReport(slots);

            Slot allSlot = new Slot(7);
            foreach (var drawing in drawings)
            {
                for (int id = 0; id < drawing.BallNumbers.Length; id++)
                {
                    allSlot.Balls[drawing.BallNumbers[id]].AddDrawing(drawing.DrawingDateDate);
                }
            }
            SaveAllReport(allSlot);

            //SaveAllPossibeNumbers(GetAllPossibleNumbers(), drawings);
        }

        public static void SaveReport(Slots data)
        {
            string filename = $"{Utilities.Path}{Utilities.GetGameName()}{NextDrawingDate.Year}.{NextDrawingDate.Month}.{NextDrawingDate.Day}.csv";
            System.IO.File.WriteAllText($"{filename}", data.Report());

            string varianceReport = $"{Utilities.Path}{Utilities.GetGameName()}-VarianceReport.csv";
            System.IO.File.WriteAllText($"{varianceReport}", data.VarianceReport());

        }

        public static void SaveAllReport(Slot mergedSlot)
        {
            string filename = $"{Utilities.Path}{Utilities.GetGameName()}-SingleSlot-{NextDrawingDate.Year}.{NextDrawingDate.Month}.{NextDrawingDate.Day}.csv";
            System.IO.File.WriteAllText($"{filename}", mergedSlot.Report());

            string varianceReport = $"{Utilities.Path}{Utilities.GetGameName()}-SingleSlot-VarianceReport.csv";
            System.IO.File.WriteAllText($"{varianceReport}", mergedSlot.VarianceReport());
        }

        //public static void SaveAllPossibeNumbers(Dictionary<int, PossibleNumbers> numbers, List<GameBalls> drawings)
        //{
        //    foreach (var drawing in drawings)
        //    {
        //        string ballspicked = BallsToString(drawing.BallNumbers[0], drawing.BallNumbers[1], drawing.BallNumbers[2], drawing.BallNumbers[3], drawing.BallNumbers[4], drawing.BallNumbers[5]);
        //        int Id = ballspicked.GetHashCode();
        //        if (numbers[Id].ChosenOn2 != null) throw new Exception("has been chosen twice!");
        //        if (numbers[Id].ChosenOn2 != null) numbers[Id] = new PossibleNumbers
        //        {
        //            Index = numbers[Id].Index,
        //            Balls = numbers[Id].Balls,
        //            ChosenOn1 = numbers[Id].ChosenOn1,
        //            ChosenOn2 = drawing.DrawingDateDate
        //        };
        //        if (numbers[Id].ChosenOn1 != null) numbers[Id] = new PossibleNumbers
        //        {
        //            Index = numbers[Id].Index,
        //            Balls = numbers[Id].Balls,
        //            ChosenOn1 = drawing.DrawingDateDate
        //        };
        //    }
        //    string filename = $"{Utilities.Path}{Utilities.GetGameName()}-AllPossibleNumbers.csv";
        //    StringBuilder sb = new StringBuilder();
        //    sb.AppendLine("Index,Balls,Date1,Date2");
        //    foreach (var item in numbers)
        //    {
        //        sb.AppendLine($"{item.Value.Index},{item.Value.Balls},{item.Value.ChosenOn1.Value.ToShortDateString()},{item.Value.ChosenOn2.Value.ToShortDateString()}");
        //    }
        //    System.IO.File.WriteAllText($"{filename}", sb.ToString());
        //}

        public static string BallsToString(int b1, int b2, int b3, int b4, int b5, int b6) => $"{b1:D2}-{b2:D2}-{b3:D2}-{b4:D2}-{b5:D2}-{b6:D2}";

        public static Dictionary<int, PossibleNumbers> GetAllPossibleNumbers()
        {
            string filename = $"{Utilities.Path}{Utilities.GetGameName()}-AllPossible-Combinations.csv";
            Dictionary<int, PossibleNumbers> numbers = new Dictionary<int, PossibleNumbers>();
            int id = 0;
            if (CurrentGame != Game.Hit5)
                for (int b1 = Utilities.GetMinMaxForSlot(0).Item1; b1 < Utilities.GetMinMaxForSlot(0).Item2; b1++)
                {
                    for (int b2 = Utilities.GetMinMaxForSlot(1).Item1; b2 < Utilities.GetMinMaxForSlot(1).Item2; b2++)
                    {
                        for (int b3 = Utilities.GetMinMaxForSlot(2).Item1; b3 < Utilities.GetMinMaxForSlot(2).Item2; b3++)
                        {
                            for (int b4 = Utilities.GetMinMaxForSlot(3).Item1; b4 < Utilities.GetMinMaxForSlot(3).Item2; b4++)
                            {
                                for (int b5 = Utilities.GetMinMaxForSlot(4).Item1; b5 < Utilities.GetMinMaxForSlot(4).Item2; b5++)
                                {
                                    for (int b6 = Utilities.GetMinMaxForSlot(5).Item1; b6 < Utilities.GetMinMaxForSlot(5).Item2; b6++)
                                    {
                                        string balls = BallsToString(b1, b2, b3, b4, b5, b6);
                                        int key = balls.GetHashCode();
                                        if (!numbers.ContainsKey(key))
                                        {
                                            numbers.Add(key,
                                                new PossibleNumbers
                                                {
                                                    Index = ++id,
                                                    Balls = BallsToString(b1, b2, b3, b4, b5, b6),
                                                    ChosenOn1 = null,
                                                    ChosenOn2 = null
                                                });
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            return numbers;
        }

        public static List<string> GetLinks(int startYear, int endYear)
        {
            string page = "http://www.walottery.com/WinningNumbers/PastDrawings.aspx";
            string gameParameter = Utilities.GetGameName();
            List<String> links = new List<string>();
            for (int i = startYear; i < endYear + 1; i++)
            {
                links.Add($"{page}?gamename={gameParameter}&unittype=year&unitcount={i}");
            }

            return links;
        }

        static List<GameBalls> SaveLoad(DateTime lastdrawingData, List<string> links)
        {
            string filename = $"{Utilities.Path}{Utilities.GetGameName()}.json";
            List<GameBalls> drawings;
            if (System.IO.File.Exists(filename))
            {
                drawings = JsonConvert.DeserializeObject<List<GameBalls>>(System.IO.File.ReadAllText(filename));
            }
            else
            {
                drawings = ScrapeWinningNumbers(links.ToArray());
                System.IO.File.WriteAllText(filename, JsonConvert.SerializeObject(drawings.OrderBy(b => b.DrawingDateDate), Formatting.Indented));
            }

            return drawings;
        }

        private static List<GameBalls> ScrapeWinningNumbers(string[] links)
        {
            var web = new HtmlWeb();
            var result = new List<GameBalls>();

            foreach (var link in links.Where(i => !string.IsNullOrEmpty(i)))
            {

                var doc = web.Load(link);
                string[] classes = new string[] { "table-viewport-large" };
                var drawingTable = doc.DocumentNode.SelectNodes($"//table[@class='table-viewport-large']");
                //var drawingDate = drawingTable.Descendants().Where(i => i.Name == "h2" && i.InnerText.CleanInnerText() != null).First();
                //var drawingBalls = drawingTable.Descendants().Where(i => i.Name == "li" && i.InnerText.CleanInnerText() != null);

                //var drawingPowerBall = drawingTable.Descendants()
                foreach (var drawing in drawingTable)
                {
                    GameBalls balls = new GameBalls()
                    {
                        DrawingDate = drawing.Descendants().Where(i => i.Name == "h2"
                        && i.InnerText.CleanInnerText() != null).First().InnerText.CleanInnerText()
                    };

                    var gameballsNodes = drawing.Descendants().Where(i => i.Name == "tbody")
                        .First<HtmlNode>().Descendants().Where(i => i.Name == "li");

                    foreach (var ball in gameballsNodes)
                    {
                        balls.AddBall(ball.InnerText.CleanInnerText());
                    };

                    var prizeNodes = drawing.Descendants().Where(i => i.Name == "tbody")
                        .First<HtmlNode>().Descendants().Where(i => i.Name == "td").ToArray();
                    for (int i = 0; i < prizeNodes.Count(); i++)
                    {
                        var item = prizeNodes[i];
                        if (item.InnerText.CleanInnerText().StartsWith("$"))
                        {
                            balls.PrizeAmount = Decimal.Parse(item.InnerText.CleanInnerText().Substring(1));
                            balls.Winners = int.Parse(prizeNodes[i + 1].InnerText.CleanInnerText());
                            break;
                        }
                    }

                    result.Add(balls);
                }
            }

            return result;
        }

    }
}

