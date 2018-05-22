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
        public string FirstLine { get; set; }
        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            string filename = $"{context.FilePath}{context.GetGameName()}-Period-PropabilityGroupsData.csv";
            SaveToCsvFile(context, filename);
        }

        public override void Execute(DrawingContext context, IEnumerable<string> additionalMetaData)
        {
            if (additionalMetaData == null) throw new NullReferenceException("must have additionalMetaData with 2 elemenets");
            if (additionalMetaData.Count() != 2) throw new ArgumentOutOfRangeException("must have 2 elements only.");

            int days = int.Parse(additionalMetaData.ToArray()[0]);
            string filename = additionalMetaData.ToArray()[1];

            context.SetDrawingsDateRange(context.EndDate.AddDays(days), context.EndDate);
            SaveToCsvFile(context, filename);
        }

        public void SaveToCsvFile(DrawingContext context, string filename)
        {
            StringBuilder sb = new StringBuilder();
            if(!string.IsNullOrEmpty(FirstLine))sb.AppendLine(FirstLine);
            sb.AppendLine("Game, Period, Slot, Propability, Numbers");
            foreach (var period in context.HistoricalGroups.PeriodGroups.Keys)
            {
                var slotGroup = context.HistoricalGroups.PeriodGroups[period];
                if (slotGroup.Count == 0) continue;

                for (int slotid = 0; slotid <= DrawingContext.GetBallCount(); slotid++)
                {
                    foreach (SubSets group in (SubSets[])Enum.GetValues(typeof(SubSets)))
                    {
                        sb.Append($"{DrawingContext.GameType}, {period}, {slotid}, {group.ToString()},")
                            .AppendLine(string.Join(",", slotGroup[slotid].Numbers(group).Select(i => i.BallId).ToArray()));
                    }
                }
            }
            System.IO.File.WriteAllText(filename, sb.ToString());
        }
    }
}
