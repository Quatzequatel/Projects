using System.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace LotteryV2.Domain.Commands
{
    class SaveGroups2JsonCommand : Command<DrawingContext>
    {
        private string _Filename;
        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SaveJsonToFileCommand");
            _Filename = $"{context.FilePath}{context.GetGameName()}-Groups.json";
            SaveToJson(context);
        }

        private void SaveToJson(DrawingContext context)
        {
            System.IO.File.WriteAllText(_Filename, 
                JsonConvert.SerializeObject(context.GroupsDictionary.Select(i => i.Value).ToArray(), 
                Formatting.Indented));
        }
    }

    class SaveDrawingTemplateToCSVCommand: Command<DrawingContext>
    {
        private string filename;

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("SaveJsonToFileCommand");
            filename = filename = $"{context.FilePath}{context.GetGameName()}_base.csv";

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

            System.IO.File.WriteAllText(filename, sb.ToString());

        }
    }

    //DefineTemplateSets
    //TBD implement AddDrawings as an extension to FingerPrint; see Templates.AdddDrawings()
    //TBD Choose top X numbers
    //TBD create every permutation of above numbers
    //TBD filter permutations against templates keeping only values in the RedHot group.
    class TemplateSetsReportCommand : Command<DrawingContext>
        {
            private string filename;
            public override void Execute(DrawingContext context)
            {
                filename = $"{context.FilePath}{context.GetGameName()}_templateSetsReport.csv";
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
                System.IO.File.WriteAllText(filename, sb.ToString());
            }
        }
}
