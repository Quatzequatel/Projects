using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace LotteryV2.Domain.Commands
{
    public class PickNumbersBaseCommand : Command<DrawingContext>
    {
        Dictionary<int, List<int>> slotPicks;

        public override bool ShouldExecute(DrawingContext context)
        {
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            GetBestCurrentPossibilities(context);
            string filename = $"{context.FilePath}{context.GetGameName()}-BestPicks-{DateTime.Now.Year}-{DateTime.Now.Month}-{DateTime.Now.Day}.csv";
            SaveToCsvFile(context, filename);
        }

        void GetBestCurrentPossibilities(DrawingContext context)
        {
            slotPicks = new Dictionary<int, List<int>>();
            for (int slotid = 1; slotid <= DrawingContext.GetBallCount(); slotid++)
            {
                //never choose numbers never selected in history.
                List<int> neverChoose = context.HistoricalGroups
                    .PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero)
                    .Select(i => i.BallId).ToList();
                //Add low choosen numbers to neverChoose list
                neverChoose.AddRange(context.HistoricalGroups
                    .PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Low)
                    .Select(i => i.BallId).ToList());

                List<int> currentPossibilities = context.HistoricalGroups
                    .PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero)
                    .Select(i => i.BallId).ToList();

                slotPicks[slotid] = currentPossibilities.Except(neverChoose).ToList();
            }
        }

        public void SaveToCsvFile(DrawingContext context, string filename)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine($"Date: {DateTime.Now.ToShortDateString()}")
                .AppendLine("Slot, Numbers,");
            for (int slotid = 1; slotid <= DrawingContext.GetBallCount(); slotid++)
            {
                sb.AppendLine($"{slotid}, {string.Join(",", slotPicks[slotid])}");
            }

            System.IO.File.WriteAllText(filename, sb.ToString());
        }

    }
}
