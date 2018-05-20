using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace LotteryV2.Domain.Commands
{
    /// <summary>
    /// Saves out the BallId's for each Slot and each SubSet.
    /// </summary>
    public class SaveGroupsDictionaryToCSVCommand : Command<DrawingContext>
    {
        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            string filename = $"{context.FilePath}{context.GetGameName()}-PropabilityGroupsData.csv";
            SaveToCsvFile(context, filename);
        }

        public override void Execute(DrawingContext context, IEnumerable<string> additionalMetaData)
        {
            if (additionalMetaData == null) throw new NullReferenceException("must have additionalMetaData with 2 elemenets");
            if (additionalMetaData.Count() != 2) throw new ArgumentOutOfRangeException("must have 2 elements only.");

            int days = int.Parse(additionalMetaData.ToArray()[0]);
            string filename = additionalMetaData.ToArray()[1];

            context.SetDrawingsDateRange(context.StartDateGet().AddDays(days), context.EndDateGet());
            SaveToCsvFile(context, filename);
        }

        public void SaveToCsvFile(DrawingContext context, string filename)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Game, Slot, Propability, Numbers");
            foreach (var slotGroup in context.GroupsDictionary)
            {
                foreach (SubSets group in (SubSets[])Enum.GetValues(typeof(SubSets)))
                {
                    sb.Append($"{context.GameType}, {slotGroup.Key}, {group.ToString()},")
                        .AppendLine(string.Join(",", slotGroup.Value.Numbers(group).Select(i => i.BallId).ToArray()));
                }
            }
            System.IO.File.WriteAllText(filename, sb.ToString());
        }
    }
}
