﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LotteryV2.Domain.Model;

namespace LotteryV2.Domain.Commands
{
    class PurmutateNumbers : Command<DrawingContext>
    {

        public override void Execute(DrawingContext context)
        {
            Dictionary<int, SlotGroup> groups = context.GroupsDictionary;
            List<int> input = new List<int>();
            //for (int i = 1; i < context.SlotCount; i++)
            //{
            //    input.AddRange(groups[i].Numbers(GroupType.High).Select(num => num.Id).ToList());
            //}

            input.AddRange(groups[1].Numbers(SubSets.High).Select(num => num.BallId).ToList());
            input.AddRange(groups[2].Numbers(SubSets.MidHigh).Select(num => num.BallId).ToList());
            input.AddRange(groups[3].Numbers(SubSets.MidHigh).Select(num => num.BallId).ToList());
            input.AddRange(groups[4].Numbers(SubSets.High).Select(num => num.BallId).ToList());
            input.AddRange(groups[5].Numbers(SubSets.High).Select(num => num.BallId).ToList());
            if(context.SlotCount > 5)input.AddRange(groups[6].Numbers(SubSets.High).Select(num => num.BallId).ToList());

            context.AddToPickedList(Groups.GenerateLotoNumbersFromInputArray(input.ToArray(), 6));

            StringBuilder sb = new StringBuilder();
            sb.Append("Source values:,").AppendLine(string.Join(", ", input.ToArray()));
            sb.AppendLine($"unique list(filtered): {context.PickedNumbers.Count} items");

            foreach (var item in context.PickedNumbers.OrderBy(i => i.Key).ToArray())
            {
                sb.Append(item.Key).Append(",").AppendLine(item.Value.Sum.ToString());
            }

            string _Filename = $"{context.FilePath}{context.GetGameName()}_PickedNumbers.csv";
            System.IO.File.WriteAllText(_Filename, sb.ToString());
        }


    }
}
