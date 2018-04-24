using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using LotteryV3.Domain.Entities;
using LotteryV3.Domain.Extensions;
using System.Text;

namespace LotteryV3.Domain.Commands
{
    internal class LoadFromFile : Command<DrawingContext>
    {
        private string _Filename;

        public override bool ShouldExecute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName}.json";
            return System.IO.File.Exists(_Filename);
        }
        public override void Execute(DrawingContext context)
        {
            LoadDrawingsFromFile(context);
        }

        void LoadDrawingsFromFile(DrawingContext context)
        {
            List<Drawing> data = JsonConvert.DeserializeObject<List<Drawing>>(System.IO.File.ReadAllText(_Filename));
            context.SetDrawings(data);
        }
    }
    internal class ScrapeFromWeb : Command<DrawingContext>
    {
        private string _Filename;


        public override bool ShouldExecute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName}.json";
            return !System.IO.File.Exists(_Filename);
        }

        public override void Execute(DrawingContext context)
        {
            ScrapeDrawings(context);
            SaveToJSON(context);
        }

        private void SaveToJSON(DrawingContext context)
        {
            System.IO.File.WriteAllText(_Filename, JsonConvert.SerializeObject(context.Drawings.OrderBy(b => b.DrawingDate), Formatting.Indented));
        }


        private void ScrapeDrawings(DrawingContext context)
        {
            var web = new HtmlWeb();
            var results = new List<Drawing>();

            foreach (var link in context.GetLinks())
            {

                var doc = web.Load(link);
                var xpath = @"//table[@class='table-viewport-large']";
                var drawingTable = doc.DocumentNode.SelectNodes(xpath);
                if (drawingTable == null) continue;
                foreach (var drawing in drawingTable)
                {
                    Drawing balls = new Drawing(context).SetDrawingDate
                        (
                       drawing.Descendants().Where(i => i.Name == "h2"
                        && i.InnerText.CleanInnerText() != null).First().InnerText.CleanInnerText()
                    );//.SetContext(context);

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
                            balls.SetPrizeAmount(Decimal.Parse(item.InnerText.CleanInnerText().Substring(1)));
                            balls.SetWinners(int.Parse(prizeNodes[i + 1].InnerText.CleanInnerText()));
                            break;
                        }
                    }

                    results.Add(balls);
                }
            }
            context.SetDrawings(results);
        }
    }

    public class UpdateJsonFromWeb : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return context?.Drawings?.Last<Drawing>()?.DrawingDate <= context?.NextDrawingDate;
        }
        public override void Execute(DrawingContext context)
        {
            ScrapeDrawings(context);
        }
        private void ScrapeDrawings(DrawingContext context)
        {
            var web = new HtmlWeb();
            var results = context.Drawings;

            foreach (var link in context.GetLinks(true))
            {

                var doc = web.Load(link);
                var xpath = @"//table[@class='table-viewport-large']";
                var drawingTable = doc.DocumentNode.SelectNodes(xpath);
                if (drawingTable == null) continue;
                foreach (var drawing in drawingTable)
                {
                    Drawing balls = new Drawing(context).SetDrawingDate
                        (
                       drawing.Descendants().Where(i => i.Name == "h2"
                        && i.InnerText.CleanInnerText() != null).First().InnerText.CleanInnerText()
                    );//.SetContext(context);

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
                            balls.SetPrizeAmount(Decimal.Parse(item.InnerText.CleanInnerText().Substring(1)));
                            balls.SetWinners(int.Parse(prizeNodes[i + 1].InnerText.CleanInnerText()));
                            break;
                        }
                    }

                    if (results.FirstOrDefault(i => i.DrawingDate == balls.DrawingDate) == null)
                    {
                        results.Add(balls);
                    }

                }
            }
            context.ReplaceDrawings(results);
        }
    }
    class DefineGroupsCommand : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            context.DefineGroups();
        }
    }

    public class SaveDrawingsJsonToFileCommand : Command<DrawingContext>
    {
        private string _Filename;

        public override void Execute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName}-DrawingsData.json";
            SaveToJSON(context);
        }

        private void SaveToJSON(DrawingContext context)
        {
            System.IO.File.WriteAllText(_Filename, JsonConvert.SerializeObject(context.Drawings.OrderBy(b => b.DrawingDate), Formatting.Indented));
        }
    }

    public class SavePropabilityGroupsJsonToFileCommand : Command<DrawingContext>
    {
        private string _Filename;

        public override void Execute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName}-PropabilityGroupsData.json";
            SaveToJSON(context);
        }

        private void SaveToJSON(DrawingContext context)
        {
            System.IO.File.WriteAllText(_Filename, JsonConvert.SerializeObject(context.GetPropabilityGroups, Formatting.Indented));
        }
    }

    public class SavePropabilityGroupsCSVToFileCommand : Command<DrawingContext>
    {
        private string _Filename;

        public override void Execute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName}-PropabilityGroupsData.CSV";
            SaveToCSV(context);
        }

        private void SaveToCSV(DrawingContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.GetPropabilityGroups.Propabilities[0].CSVHeading);
            foreach (var item in context.GetPropabilityGroups.Propabilities)
            {
                sb.AppendLine(item.CSVLine);
            }

            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }
    }

    public class SaveDrawingsToCSVFileCommand : Command<DrawingContext>
    {
        private string _Filename;
        public override void Execute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName}-DrawingsData.csv";
            SaveToCSV(context);
        }
        private void SaveToCSV(DrawingContext context)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(context.Drawings[0].CSVHeading);
            foreach (var item in context.Drawings)
            {
                sb.Append(item.ToCSVString).Append(",").Append(item.GetDrawingPattern().Select(i=> i.ToString()).ToArray().CSV()).AppendLine();
            }

            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }
    }

}