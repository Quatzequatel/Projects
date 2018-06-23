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
                    .OrderByDescending(i => i.DrawingsCount)
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
            AllChoices(filename);
        }

        public void AllChoices(string filename)
        {
            for (int slotid = 1; slotid <= DrawingContext.GetBallCount(); slotid++)
            {
                for (int numberId = 0; numberId < slotPicks[slotid].Count; numberId++)
                {
                    StringBuilder sbNextSet = new StringBuilder();
                    sbNextSet.Append($"{slotPicks[slotid][numberId]}");
                    buildPick(slotid + 1, sbNextSet, filename);
                }
            }
        }

        public void buildPick(int slotid, StringBuilder sb, string filename)
        {
            if (slotid >= DrawingContext.GetBallCount())
            {
                string firstPartOfChoice = sb.ToString();
                for (int i = 0; i < slotPicks[slotid].Count(); i++)
                {
                    if (i == 0)
                        sb.AppendLine($"-{slotPicks[slotid][i]}");
                    else
                    {
                        sb.AppendLine($"{firstPartOfChoice}-{slotPicks[slotid][i]}");
                    }
                }
                //Save data to file.
                System.IO.File.AppendAllText(filename, sb.ToString());
                sb = new StringBuilder();
            }
            else
            {
                string firstPartOfChoice = sb.ToString();

                for (int id = 0; id < slotPicks[slotid].Count(); id++)
                {
                    StringBuilder sbNext = new StringBuilder(firstPartOfChoice);
                    sbNext.Append($"-{slotPicks[slotid][id]}");
                    buildPick(slotid + 1, sbNext, filename);
                }
            }
        }
    }
}
