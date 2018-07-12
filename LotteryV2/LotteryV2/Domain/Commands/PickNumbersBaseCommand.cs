using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;


namespace LotteryV2.Domain.Commands
{
    public class PickNumbersBaseCommand : Command<DrawingContext>
    {
        Dictionary<int, List<int>> slotPicks;
        DrawingContext context;

        public override bool ShouldExecute(DrawingContext context)
        {
            this.context = context;
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
                //neverChoose.AddRange(context.HistoricalGroups
                //    .PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Low)
                //    .Select(i => i.BallId).ToList());

                List<int> currentPossibilities = CurrentBestNumbers(context, slotid);


                if (context.HasOptionalBall() && (slotid == DrawingContext.GetBallCount()))
                {
                    slotPicks[slotid] = currentPossibilities.Except(neverChoose).ToList()
                        .Where(i => i < context.OptionBallHighNumber()).ToList();
                }
                else
                {
                    slotPicks[slotid] = currentPossibilities.Except(neverChoose).ToList();
                }
            }
        }

        private List<int> CurrentBestNumbers(DrawingContext context, int slotid)
        {
            if (DrawingContext.GameType == Game.MegaMillion) return CurrentBestNumbersMegaMillion(context, slotid);
            if (DrawingContext.GameType == Game.Lotto) return CurrentBestNumbersLotto(context, slotid);

            List<int> never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();
            List<int> Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();
            List<int> All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();

            //List<int> zeroList = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            //    //.Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)).ToList();
            //List<int> highList = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();
            List<int> probablrGroup = wk1.Intersect(wk2).Except(never).ToList();
            //result1 = result1.Intersect(wk2).ToList();
            probablrGroup = probablrGroup.Intersect(wk3).Except(never).ToList();
            probablrGroup = probablrGroup.Intersect(Month1).ToList();
            probablrGroup = probablrGroup.Intersect(Month2).ToList();
            probablrGroup = probablrGroup.Except(never).ToList();

            List<int> highgroup = All.Union(Year).Union(Month6).RandomPermutation().ToList();

            List<int> result = probablrGroup.Intersect(highgroup).ToArray().RandomPermutation().Take(5).ToList();

            if (result.Count() == 0)
            {
                highgroup = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)
                    .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId))
                    .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)).ToList();

                result = wk1.Intersect(wk2).Intersect(highgroup).RandomPermutation().Take(5).ToList();
            }


            //return result.Count() > 2 ? result : CurrentBestNumbers2(context, slotid);
            return result.Count() == 0 ? highgroup : result;

        }

        private List<int> CurrentBestNumbersLotto(DrawingContext context, int slotid)
        {
            List<int> never = new List<int>();
            List<int> wk1 = new List<int>();
            List<int> wk2 = new List<int>();
            List<int> wk3 = new List<int>();
            List<int> m2 = new List<int>();
            List<int> m3 = new List<int>();
            List<int> Month6 = new List<int>();
            List<int> Year = new List<int>();
            List<int> All = new List<int>();

            switch (slotid)
            {
                case 1:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 =  context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).Select(i => i.BallId)).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i=> i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 2:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                        
                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();

                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;
                case 3:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 4:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();

                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 5:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 6:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Low).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).Select(i => i.BallId)).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)).ToList();

                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId).ToList();
                    break;
                default:
                    break;
            }


            List<int> probablrGroup = (wk1.Intersect(wk2)).Intersect(wk3).ToList();
            probablrGroup = probablrGroup.Intersect(m2).ToList();
            probablrGroup = probablrGroup.Intersect(m3).ToList();
            probablrGroup = probablrGroup.Except(never).ToList();

            List<int> highgroup = All.Union(Year).Union(Month6).RandomPermutation().ToList();

            List<int> result = probablrGroup.Intersect(highgroup).RandomPermutation().ToList();

            if (result.Count() == 0)
            {
                highgroup = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)
                    .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId))
                    .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)).ToList();

                result = probablrGroup.Intersect(highgroup).RandomPermutation().ToList();
            }


            //return result.Count() > 2 ? result : CurrentBestNumbers2(context, slotid);
            return result.Count() == 0 ? Month6.RandomPermutation().Take(5).ToList() : result.Take(5).ToList();
        }


        private List<int> CurrentBestNumbersMegaMillion(DrawingContext context, int slotid)
        {
            List<int> never = new List<int>();
            List<int> wk1 = new List<int>();
            List<int> wk2 = new List<int>();
            List<int> wk3 = new List<int>();
            List<int> m2 = new List<int>();
            List<int> m3 = new List<int>();
            List<int> Month6 = new List<int>();
            List<int> Year = new List<int>();
            List<int> All = new List<int>();

            switch (slotid)
            {
                case 1:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).Select(i => i.BallId)).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 2:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();

                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;
                case 3:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 4:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();

                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 5:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();
                    break;

                case 6:
                    never = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Low).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).Select(i => i.BallId)).ToList();

                    Month6 = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).Select(i => i.BallId)).ToList();

                    Year = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).Select(i => i.BallId)).ToList();

                    All = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).Select(i => i.BallId).ToList();
                    break;
                default:
                    break;
            }


            List<int> probablrGroup = (wk1.Intersect(wk2)).Intersect(wk3).ToList();
            probablrGroup = probablrGroup.Intersect(m2).ToList();
            probablrGroup = probablrGroup.Intersect(m3).ToList();
            probablrGroup = probablrGroup.Except(never).ToList();

            List<int> highgroup = All.Union(Year).Union(Month6).RandomPermutation().ToList();

            List<int> result = probablrGroup.Intersect(highgroup).RandomPermutation().ToList();

            if (result.Count() == 0)
            {
                highgroup = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)
                    .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId))
                    .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)).ToList();

                result = probablrGroup.Intersect(highgroup).RandomPermutation().ToList();
            }


            //return result.Count() > 2 ? result : CurrentBestNumbers2(context, slotid);
            return result.Count() == 0 ? Month6.RandomPermutation().ToList() : result.Take(5).ToList();
        }
        private List<int> CurrentBestNumbers2(DrawingContext context, int slotid)
        {
            List<int> zeroList = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)).ToList();
            List<int> highList = context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount)
                .Union(context.HistoricalGroups.PeriodGroups[HistoricalPeriods.All][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount))
                .Select(i => i.BallId).ToList();

            List<int> result = highList.Intersect(zeroList).RandomPermutation().ToList();

            return result;
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
            sb.AppendLine().AppendLine().AppendLine("Numbers, Base13, Base21");
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
                    BuildPick(slotid + 1, sbNextSet, filename);
                }
            }
        }

        public void BuildPick(int slotid, StringBuilder sb, string filename)
        {
            if (slotid > DrawingContext.GetBallCount()) return;

            if (slotid == DrawingContext.GetBallCount())
            {
                string firstPartOfChoice = sb.ToString();
                sb = new StringBuilder();
                for (int i = 0; i < slotPicks[slotid].Count(); i++)
                {
                    string[] nums = firstPartOfChoice.Split('-');
                    if (nums[nums.Count() - 1] != slotPicks[slotid][i].ToString())
                    {
                        if ($"{firstPartOfChoice}-{slotPicks[slotid][i]}".Split('-').Count() == DrawingContext.GetBallCount())
                        {
                            sb.AppendLine($"{firstPartOfChoice}-{slotPicks[slotid][i]}");
                        }
                    }
                }

                //Save data to file.
                System.IO.File.AppendAllText(filename, sb.ToString());
                sb = new StringBuilder();
            }
            else
            {
                string firstPartOfChoice = sb.ToString();
                string[] numbers = firstPartOfChoice.Split('-');
                int min = int.Parse(numbers[numbers.Length - 1]);

                for (int id = 0; id < slotPicks[slotid].Count(); id++)
                {
                    if (context.HasOptionalBall() || slotPicks[slotid][id] != min)
                    {
                        StringBuilder sbNext = new StringBuilder(firstPartOfChoice);
                        sbNext.Append($"-{slotPicks[slotid][id]}");
                        BuildPick(slotid + 1, sbNext, filename);
                    }
                }
            }
        }
    }
}
