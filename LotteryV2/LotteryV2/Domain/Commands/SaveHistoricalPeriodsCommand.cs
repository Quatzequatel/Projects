﻿using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;
using LotteryV2.Domain.Model;

namespace LotteryV2.Domain.Commands
{
    public class SaveHistoricalPeriodsCommand : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SaveHistoricalPeriodsCommand");
            SaveToJSON(context);
        }

        private void SaveToJSON(DrawingContext context)
        {
            List<object> results = new List<object>();

            foreach (var item in context.AllDrawings.OrderBy(d => d.DrawingDate))
            {
                string temp = JsonConvert.SerializeObject(item.HistoricalPeriodFingerPrints, Formatting.Indented);

                results.Add(
                    new HistoricalPeriodsJson()
                    {
                        DrawingDate = item.DrawingDate,
                        Numbers = item.Numbers,
                        KeyString = item.KeyString,
                        JsonHistoricalFingerPrints = item.JsonHistoricalFingerPrints
                    });
            }

            System.IO.File.WriteAllText(context.FileHistoricalPeriods, JsonConvert.SerializeObject(results, Formatting.Indented));
        }
    }

    public class SetHistoricalPeriodsCommand: Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }
        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SetHistoricalPeriods");
            context.SetHistoricalPeriods();
        }
    }
}
