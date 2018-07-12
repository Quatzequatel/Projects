using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryV2.Domain.Commands
{
    public class SingleCommand
    {
        private DrawingContext context;

        public void Execute( DrawingContext context)
        {
            this.context = context;
            this.Execute();

        }
        public void Execute()
        {

            //DefineDateRange
            context.SetDrawingsDateRange(System.DateTime.Now.AddMonths(-60)
                , new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day));
            Console.WriteLine($"Begin type {context.GetGameName()} DateRange: {context.StartDate} to {context.EndDate}");

            //LoadDrawingsFromFile
            string filename = $"{context.FilePath}{context.GetGameName()}.json";
            if (System.IO.File.Exists(filename))
            {
                Console.WriteLine("LoadDrawingsFromFile");
                List<Drawing> data = JsonConvert.DeserializeObject<List<Drawing>>(System.IO.File.ReadAllText(filename));
                context.SetDrawings(data);
            }

            //ScrapeFromWeb
            if (!System.IO.File.Exists(filename))
            {
                Console.WriteLine("ScrapeFromWeb");
                ScrapeFromWeb command = new ScrapeFromWeb();
                command.ShouldExecute(context);
                //command.Execute(context);
            }

            //UpdateJsonFromWeb
            using (UpdateJsonFromWeb c = new UpdateJsonFromWeb())
            {
                Console.WriteLine("UpdateJsonFromWeb");
                if (c.ShouldExecute(context))
                {
                    //c.Execute(context);
                }
            }

            //SaveToJson
            using (SaveJsonToFileCommand c = new SaveJsonToFileCommand())
            {
                Console.WriteLine("SaveJsonToFileCommand");
                if (c.ShouldExecute(context))
                {
                    //c.Execute(context);
                }
            }

            bool shouldExecute = true;
            bool shouldExecuteSetHistoricalPeriods = true;
            LoadFilehistoricalPeriods loadHistoricalPeriods = new LoadFilehistoricalPeriods();
            if (loadHistoricalPeriods.ShouldExecute(context))
            {
                Console.WriteLine("LoadFilehistoricalPeriods");
                loadHistoricalPeriods.Execute(context);
                shouldExecuteSetHistoricalPeriods = false;
            }

            using (DefineHistoricalGroups c = new DefineHistoricalGroups())
            {
                Console.WriteLine("DefineHistoricalGroups");
                if (c.ShouldExecute(context))
                {
                    c.Execute(context);
                }
            }

            using (SaveGroupsDictionaryToCSVCommand c = new SaveGroupsDictionaryToCSVCommand())
            {
                Console.WriteLine("SaveGroupsDictionaryToCSVCommand");
                if (c.ShouldExecute(context))
                {
                    c.Execute(context);
                }
            }


            //Set the historical finger prints for all drawings
            if (shouldExecuteSetHistoricalPeriods)
            {
                Console.WriteLine("SetHistoricalPeriods");
                context.SetHistoricalPeriods();
            }

            /*
             Todo: 
                1. find top patterns for each period.
                2. Select numbers for each period by group.
                3. Distill numbers to final selections.
             */
            Console.WriteLine("SetTopPatternsCommand");
            (new SetTopPatternsCommand()).Execute(context);

            //SaveToJson
            SaveHistoricalPeriodsCommand saveHistoricalPeriods2json = new SaveHistoricalPeriodsCommand();
            if (saveHistoricalPeriods2json.ShouldExecute(context))
            {
                Console.WriteLine("SaveHistoricalPeriodsCommand");
                saveHistoricalPeriods2json.Execute(context);
            }

            //Validate last 100 drawing against Patterns
            using (PastDrawingReportCommand c = new PastDrawingReportCommand())
            {
                if (c.ShouldExecute(context))
                {
                    c.Execute(context);
                }
            }

            //Dump out all of the templates by period to CSV.
            using (SaveHistoricalPatternSummary c = new SaveHistoricalPatternSummary())
            {
                if (c.ShouldExecute(context))
                {
                    Console.WriteLine("SaveHistoricalPatternSummary");
                    c.Execute(context);
                }
            }

            //Get Best numbers to pick.
            using (PickNumbersBaseCommand c = new PickNumbersBaseCommand())
            {
                if (c.ShouldExecute(context))
                {
                    Console.WriteLine("PickNumbersBaseCommand");
                    c.Execute(context);
                }
            }

            //SaveGroups2JsonCommand
            if (shouldExecute)
            {
                filename = $"{context.FilePath}{context.GetGameName()}-Groups.json";
                System.IO.File.WriteAllText(
                    filename,
                    JsonConvert.SerializeObject(context.GroupsDictionary.Select(i => i.Value).ToArray(),
                    Formatting.Indented));
            }
            string _Filename;
            //SaveGroups2JsonCommand
            if (shouldExecute)
            {
                _Filename = $"{context.FilePath}{context.GetGameName()}-Groups.json";
                System.IO.File.WriteAllText(
                    _Filename,
                    JsonConvert.SerializeObject(context.GroupsDictionary.Select(i => i.Value).ToArray(),
                    Formatting.Indented)
                    );
            }
            //SaveJsonToFileCommand
            if (context.Drawings.Count > 5)
            {
                Console.WriteLine("SaveJsonToFileCommand");
                _Filename = $"{context.FilePath}{context.GetGameName()}_base.csv";
                Dictionary<int, SlotGroup> groups = context.GroupsDictionary;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(context.Drawings[0].CSVHeading + ", Group");
                foreach (var item in context.Drawings)
                {
                    sb.Append(item.ToCSVString());
                    for (int i = 0; i < context.SlotCount; i++)
                    {
                        sb.Append($",{groups[i + 1].FindGroupTypes(item.Numbers[i])}");
                    }
                    sb.AppendLine();
                }

                System.IO.File.WriteAllText(_Filename, sb.ToString());
            }
            //DefineTemplateSets
            //TBD implement AddDrawings as an extension to FingerPrint; see Templates.AdddDrawings()
            //TBD Choose top X numbers
            //TBD create every permutation of above numbers
            //TBD filter permutations against templates keeping only values in the RedHot group.
            if (shouldExecute)
            {
                _Filename = $"{context.FilePath}{context.GetGameName()}_templageInfo.csv";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("key, count, template");
                foreach (var item in context.Drawings
                    .GroupBy(i => i.TemplateFingerPrint.GetValue())
                    .Select(group => new { key = group.Key, count = group.Count() })
                    .OrderBy(x => x.count))
                {
                    sb.AppendLine($"{item.key}, {item.count}, {context.Drawings.Where(i => i.TemplateFingerPrint.GetValue() == item.key).First().TemplateFingerPrint.ToString()}");
                }
                sb.AppendLine();
                System.IO.File.WriteAllText(_Filename, sb.ToString());
            }

            //SlotNumberAnalysis2CSVCommand
            using (SlotNumberAnalysis2CSVCommand c = new SlotNumberAnalysis2CSVCommand())
            {
                if (c.ShouldExecute(context))
                {
                    Console.WriteLine("SlotNumberAnalysis2CSVCommand");
                    c.Execute(context);
                }
            }
            //SlotNumberAnalysisRanged2CSVCommand
            using (SlotNumberAnalysis2CSVCommand c = new SlotNumberAnalysis2CSVCommand())
            {
                if (c.ShouldExecute(context))
                {
                    Console.WriteLine("SlotNumberAnalysis2CSVCommand");
                    c.Execute(context);
                }
            }
            using (SlotNumberAnalysisRanged2CSVCommand c = new SlotNumberAnalysisRanged2CSVCommand())
            {
                if (c.ShouldExecute(context))
                {
                    Console.WriteLine("SlotNumberAnalysisRanged2CSVCommand");
                    c.Filename = $"{context.FilePath}{context.GetGameName()}_RangedSlotNumberAnalysis.csv";
                    c.TakeDrawwings = 1000;
                    c.LeaveDrawings = 10;

                    c.Execute(context);
                }
            }
        }
    }
}
