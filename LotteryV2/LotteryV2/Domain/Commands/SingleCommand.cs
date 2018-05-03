using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LotteryV2.Domain.Commands
{
    public class SingleCommand
    {
        public void Execute()
        {
            DrawingContext context = new DrawingContext(
                Game.Hit5,
                new System.DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day)
                );

            //LoadDrawingsFromFile
            string filename = $"{context.FilePath}{context.GetGameName()}.json";
            if (System.IO.File.Exists(filename))
            {
                List<Drawing> data = JsonConvert.DeserializeObject<List<Drawing>>(System.IO.File.ReadAllText(filename));
                context.SetDrawings(data);
            }

            //ScrapeFromWeb
            if (!System.IO.File.Exists(filename))
            {
                ScrapeFromWeb command = new ScrapeFromWeb();
                command.ShouldExecute(context);
                command.Execute(context);
            }

            //UpdateJsonFromWeb
            if (context.LastDrawingDate.Value < context?.NextDrawingDate)
            {
                UpdateJsonFromWeb command = new UpdateJsonFromWeb();
                command.ShouldExecute(context);
                command.Execute(context);
            }

            //DefineDateRange
            context.SetDrawingsDateRange(new DateTime(System.DateTime.Now.Year - 1, System.DateTime.Now.Month, System.DateTime.Now.Day)
                , new DateTime(System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day));

            //DefineGroupsCommand
            bool shouldExecute = true;
            if (shouldExecute)
            {
                context.DefineGroups();
                context.Drawings.ForEach(i => i.SetContext(context));
            }
            //SetTemplateFingerPrintCommand
            if (shouldExecute)
            {
                context.Drawings.ForEach(i => i.GetTemplateFingerPrint());
            }
            //SaveGroupsToCsv
            if (shouldExecute)
            {
                StringBuilder sb = new StringBuilder();
                foreach (var slotGroup in context.GroupsDictionary)
                {
                    //sb.AppendLine($"{context.GameType},{slotGroup.Key},{slotGroup.Value.}");
                    sb.AppendLine("Game, Slot, Propability, Numbers");
                    foreach (SubSets group in (SubSets[])Enum.GetValues(typeof(SubSets)))
                    {
                        sb.Append($"{context.GameType}, {slotGroup.Key}, {group.ToString()},")
                            .Append(string.Join(",", slotGroup.Value.Numbers(group).Select(i => i.Id).ToArray()));
                        sb.AppendLine();
                    }
                }
                filename = $"{context.FilePath}{context.GetGameName()}-PropabilityGroupsData.csv";
                System.IO.File.WriteAllText(filename, sb.ToString());
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
            if (context.Drawings.Count > 500)
            {
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
                    sb.AppendLine( $"{item.key}, {item.count}, {context.Drawings.Where(i=> i.TemplateFingerPrint.GetValue() == item.key).First().TemplateFingerPrint.ToString()}");
                }
                sb.AppendLine();
                System.IO.File.WriteAllText(_Filename, sb.ToString());
            }

            //SlotNumberAnalysis2CSVCommand
            if (shouldExecute)
            {
                _Filename = _Filename = $"{context.FilePath}{context.GetGameName()}_SlotNumberAnalysis.csv";
                List<NumberModel> numbers = context.NumberModelList;
                Dictionary<int, SlotGroup> groups = context.GroupsDictionary;

                StringBuilder sb = new StringBuilder();
                sb.AppendLine(numbers[0].CSVHeading + ",GroupType");
                foreach (var item in numbers)
                {
                    sb.AppendLine(item.CSVLine + $",{groups[item.SlotId].FindGroupType(item.Id)}");
                }

                System.IO.File.WriteAllText(_Filename, sb.ToString());
            }
            //SlotNumberAnalysisRanged2CSVCommand
            if (shouldExecute)
            {
                _Filename = _Filename = $"{context.FilePath}{context.GetGameName()}_RangedSlotNumberAnalysis.csv";
                List<NumberModel> numbers = context.NumberModelList;
                int _TakeDrawwings = 1000;
                int _LeaveDrawings = 10;
                List<Tuple<int, double>> dataBag = new List<Tuple<int, double>>();

                for (int slot = 1; slot <= context.SlotCount; slot++)
                {
                    for (int number = 1; number <= context.HighestBall; number++)
                    {
                        var element = new NumberModel(number, slot, context.GameType);
                        element.LoadLastNumberOfDrawingsAndLeave(context.Drawings, _TakeDrawwings, _LeaveDrawings);
                        numbers.Add(element);
                    }

                }

                for (int number = 1; number <= context.HighestBall; number++)
                {
                    var element = new NumberModel(number, 0, context.GameType);

                    foreach (var item in numbers.Where(num => num.Id == number).ToArray())
                    {
                        if (element.DrawingsCount == 0) element.SetDrawingsCount(item.DrawingsCount);
                        element.AddDrawingDates(item.DrawingDates);
                    }
                    numbers.Add(element);
                }

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Next Numbers");
                for (int i = 1; i < _LeaveDrawings; i++)
                {
                    sb.AppendLine(context.Drawings[context.Drawings.Count - i].ToString());
                }
                sb.AppendLine().AppendLine().AppendLine(numbers[0].CSVHeading);
                foreach (var item in numbers)
                {
                    sb.AppendLine(item.CSVLine);
                }

                System.IO.File.WriteAllText(_Filename, sb.ToString());
            }
        }
    }
}
