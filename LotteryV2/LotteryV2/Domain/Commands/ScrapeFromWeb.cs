using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using LotteryV2.Domain.Model;

namespace LotteryV2.Domain.Commands
{
    internal class ScrapeFromWeb : Command<DrawingContext>
    {
        private string _Filename;

        /// <summary>
        /// Should only execute when there is no json file.
        /// </summary>
        /// <param name="context"></param>
        /// <returns>true when no json file exists</returns>
        public override bool ShouldExecute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}.json";
            return !context.SkipScrapeFromWeb; //&& System.IO.File.Exists(_Filename);
        }

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("ScrapeFromWeb");
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
}
