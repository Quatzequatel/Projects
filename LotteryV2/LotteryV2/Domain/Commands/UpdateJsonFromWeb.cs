using HtmlAgilityPack;
using ScrapySharp.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace LotteryV2.Domain.Commands
{
    public class UpdateJsonFromWeb : Command<DrawingContext>
    {
        /// <summary>
        /// only executes when context.NextDrawingDate is greater than last drawing date.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override bool ShouldExecute(DrawingContext context)
        {
            return (context.LastDrawingDate.HasValue ? context.FirstDrawingDate.Value < context?.NextDrawingDate: true);
        }
        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("UpdateJsonFromWeb");
            ScrapeDrawings(context);
        }
        private void ScrapeDrawings(DrawingContext context)
        {
            var web = new HtmlWeb();
            var results = context.AllDrawings;

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

                    ///if ball is not already in the list, add it.
                    if(results.FirstOrDefault(i=>i.DrawingDate == balls.DrawingDate) == null)
                    {
                        results.Add(balls);
                    }
                    else
                    {
                        break;
                    }
                    
                }
            }
            //replace drawings with results.
            context.ReplaceDrawings(results);
        }
    }
}
