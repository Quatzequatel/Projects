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
        private readonly int takeValue = 5;

        public override bool ShouldExecute(DrawingContext context)
        {
            this.context = context;
            return base.ShouldExecute(context);
        }

        public override void Execute(DrawingContext context)
        {
            Console.WriteLine("PickNumbersBaseCommand");
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

                List<int> currentPossibilities = CurrentBestNumbers3(context, slotid);


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

            // put all of the slotPicks[] selection into one array and remove duplicates.
            // then balance out the arrays to all have same number of elements.
            // then replace slotpicks[] witn new arrays.
            List<int> picks = new List<int>();
            int size = 5;
            //picks.AddRange(slotPicks[1]);

            //Do not add odd ball to balanced list.
            int slotCount = context.HasOptionalBall() ? DrawingContext.GetBallCount() - 1 : DrawingContext.GetBallCount();

            for (int slotid = 1; slotid <= slotCount; slotid++)
            {
                picks = picks.Union(slotPicks[slotid]).OrderBy(i => i).ToList();
            }
            picks.OrderBy(i => i);
            size = (int)Math.Floor((double)((picks.Count() + 1) / slotCount));
            for (int slotid = 1; slotid <= slotCount; slotid++)
            {
                slotPicks[slotid] = picks.Skip((slotid - 1) * size).Take(size).RandomPermutation().ToList();
            }

        }

        private List<int> CurrentBestNumbers(DrawingContext context, int slotid)
        {
            if (DrawingContext.GameType == Game.MegaMillion) return CurrentBestNumbersPowerBall(context, slotid);
            if (DrawingContext.GameType == Game.Lotto) return CurrentBestNumbersLotto(context, slotid);
            if (DrawingContext.GameType == Game.Powerball) return CurrentBestNumbersMegaMillion2(context, slotid);

            Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> data = context.HistoricalGroups.PeriodGroups;

            List<int> never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month1 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month2 = data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            List<int> Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();
            List<int> Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();
            List<int> All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();

            //List<int> zeroList = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            //    //.Union(data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)).ToList();
            //List<int> highList = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId).ToList();
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
                highgroup = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId))
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)).ToList();

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
            List<int> m1 = new List<int>();
            List<int> m2 = new List<int>();
            List<int> m3 = new List<int>();
            List<int> Month6 = new List<int>();
            List<int> Year = new List<int>();
            List<int> All = new List<int>();

            List<int> RecentGroup = new List<int>();
            List<int> LongGroup;
            List<int> Result = new List<int>();

            Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> data = context.HistoricalGroups.PeriodGroups;

            switch (slotid)
            {
                case 1:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList()
                        .Union(data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Low).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    m1 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Low).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidLow).Select(i => i.BallId)).ToList();


                    m2 = data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();



                    break;

                case 2:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m1 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Low).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidLow).Select(i => i.BallId)).ToList();

                    m2 = data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Low).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidLow).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidHigh).Select(i => i.BallId)).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId).ToList()
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        //.Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).OrderBy(i=>i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    break;
                case 3:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();
                    break;

                case 4:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    break;

                case 5:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    break;

                case 6:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Low).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderBy(i => i.PercentChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).OrderBy(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderBy(i => i.PercentChosen).Select(i => i.BallId).ToList();

                    break;
            }

            RecentGroup = (wk1.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
            LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

            Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();

            //return result.Count() > 2 ? result : CurrentBestNumbers2(context, slotid);
            return Result.Count() == 0 ? Month6.RandomPermutation().Take(8).ToList() : Result.Take(9).ToList();
        }


        private List<int> CurrentBestNumbersMegaMillion(DrawingContext context, int slotid)
        {
            List<int> never = new List<int>();
            List<int> wk1 = new List<int>();
            List<int> wk2 = new List<int>();
            List<int> wk3 = new List<int>();
            List<int> m1 = new List<int>();
            List<int> m2 = new List<int>();
            List<int> m3 = new List<int>();
            List<int> Month6 = new List<int>();
            List<int> Year = new List<int>();
            List<int> All = new List<int>();

            List<int> RecentGroup = new List<int>();
            List<int> LongGroup;
            List<int> Result = new List<int>();

            Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> data = context.HistoricalGroups.PeriodGroups;

            switch (slotid)
            {
                case 1:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList()
                        .Union(data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    m1 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidLow).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();


                    m2 = data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();

                    break;

                case 2:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m1 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Low).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidLow).Select(i => i.BallId)).ToList();

                    m2 = data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Low).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidLow).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId).ToList()
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        //.Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;
                case 3:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;

                case 4:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;

                case 5:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;

                case 6:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();

                    break;
                default:
                    break;
            }


            if (Result.Count() == 0)
            {
                List<int> midGroup;
                midGroup = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId))
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)).ToList();

                Result = midGroup.Intersect(RecentGroup).RandomPermutation().ToList();
            }

            return Result.Count() == 0 ? Month6.RandomPermutation().ToList() : Result.Take(9).ToList();
        }

        private List<int> CurrentBestNumbersMegaMillion2(DrawingContext context, int slotid)
        {
            List<int> never = new List<int>();
            List<int> wk1 = new List<int>();
            List<int> wk2 = new List<int>();
            List<int> wk3 = new List<int>();
            List<int> m1 = new List<int>();
            List<int> m2 = new List<int>();
            List<int> m3 = new List<int>();
            List<int> Month6 = new List<int>();
            List<int> Year = new List<int>();
            List<int> All = new List<int>();

            List<int> RecentGroup = new List<int>();
            List<int> LongGroup;
            List<int> Result = new List<int>();

            Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> data = context.HistoricalGroups.PeriodGroups;

            never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
            wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                .Union(data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();


            Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId).ToList();


            RecentGroup = wk1.Except(never).ToList();
            LongGroup = Month6.Except(never).ToList();

            Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();

            if (Result.Count() == 0)
            {
                List<int> midGroup;
                midGroup = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId))
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)).ToList();

                Result = midGroup.Intersect(RecentGroup).RandomPermutation().ToList();
            }

            return Result.Count() == 0 ? Month6.RandomPermutation().ToList() : Result.Take(9).ToList();
        }

        private List<int> CurrentBestNumbersPowerBall(DrawingContext context, int slotid)
        {
            List<int> never = new List<int>();
            List<int> wk1 = new List<int>();
            List<int> wk2 = new List<int>();
            List<int> wk3 = new List<int>();
            List<int> m1 = new List<int>();
            List<int> m2 = new List<int>();
            List<int> m3 = new List<int>();
            List<int> Month6 = new List<int>();
            List<int> Year = new List<int>();
            List<int> All = new List<int>();

            List<int> RecentGroup = new List<int>();
            List<int> LongGroup;
            List<int> Result = new List<int>();

            Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> data = context.HistoricalGroups.PeriodGroups;

            switch (slotid)
            {
                case 1:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList()
                        .Union(data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    m1 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidLow).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();


                    m2 = data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.PercentChosen).Select(i => i.BallId)).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();

                    break;

                case 2:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m1 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Low).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.MidLow).Select(i => i.BallId)).ToList();

                    m2 = data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Low).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidLow).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId).ToList()
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        //.Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;
                case 3:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;

                case 4:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;

                case 5:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();
                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();
                    break;

                case 6:
                    never = data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk1 = data[HistoricalPeriods.Wk1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk2 = data[HistoricalPeriods.Wk2][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();
                    wk3 = data[HistoricalPeriods.Wk3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m2 = data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero).Select(i => i.BallId).ToList();

                    m3 = data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Low).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Month6 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    Year = data[HistoricalPeriods.Year][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId))
                        .Union(data[HistoricalPeriods.Year][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId)).ToList();

                    All = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High).OrderByDescending(i => i.TimesChosen).Select(i => i.BallId).ToList();

                    RecentGroup = (wk2.Union(wk2)).Union(wk3).Union(m1).Except(never).ToList();
                    LongGroup = m2.Union(m3).Union(Month6).Except(never).Union(All).ToList();

                    Result = RecentGroup.Intersect(LongGroup).RandomPermutation().ToList();

                    break;
                default:
                    break;
            }


            if (Result.Count() == 0)
            {
                List<int> midGroup;
                midGroup = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.MidHigh).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Mid).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId))
                    .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.High).OrderByDescending(i => i.DrawingsCount).Select(i => i.BallId)).ToList();

                Result = midGroup.Intersect(RecentGroup).RandomPermutation().ToList();
            }

            return Result.Count() == 0 ? Month6.RandomPermutation().ToList() : Result.Take(9).ToList();
        }
        private List<int> CurrentBestNumbers3(DrawingContext context, int slotid)
        {

            Dictionary<HistoricalPeriods, Dictionary<int, SlotGroup>> data = context.HistoricalGroups.PeriodGroups;

            var choice2 = data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Zero)
                            .Union(data[HistoricalPeriods.Month6][slotid].Numbers(SubSets.Low))
                        .IntersectNumberModel(
                            data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Zero)
                                    .Union(data[HistoricalPeriods.Month3][slotid].Numbers(SubSets.Low))
                                )
                        .IntersectNumberModel(
                            data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Zero)
                                    .Union(data[HistoricalPeriods.Month2][slotid].Numbers(SubSets.Low))
                        )
                        .Intersect(
                            data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Zero)
                            .Union(data[HistoricalPeriods.Month1][slotid].Numbers(SubSets.Low)),
                            o => o.BallId, 
                            i => i.BallId, 
                            (o, i) => o
                            );


            choice2 = choice2.Except(data[HistoricalPeriods.All][slotid].Numbers(SubSets.Zero));

            var highChoices = data[HistoricalPeriods.All][slotid].Numbers(SubSets.High);

            highChoices = slotid != 6 ? highChoices.Union(data[HistoricalPeriods.All][slotid].Numbers(SubSets.MidHigh)).ToList() :
                highChoices.Union(data[HistoricalPeriods.All][slotid].Numbers(SubSets.Mid)).ToList();


            var choice1 = highChoices.IntersectNumberModel(choice2);
            return choice1.Union(choice2.Skip(choice1.Count())).OrderByDescending(i => i.DrawingsCount).Select(i=> i.BallId).Take(takeValue).ToList();
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
