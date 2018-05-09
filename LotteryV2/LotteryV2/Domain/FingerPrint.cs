using LotteryV2.Domain.Commands;
using System.Collections.Generic;
using System.Linq;
using System;

namespace LotteryV2.Domain
{
    public class FingerPrint
    {
        private List<SubSets> Template { get; set; } = new List<SubSets>();
        private int Value { get; set; }
        public int Count { get; set; }
        public TemplateSets TemplateSet { get; set; }
        public FingerPrint(Drawing drawing)
        {

            for (int SlotId = 1; SlotId <= drawing.Numbers.Length; SlotId++)
            {
                SubSets slotset = drawing.Context.GroupsDictionary[SlotId]?.FindGroupType(drawing.Numbers[SlotId - 1]) != null ?
                    drawing.Context.GroupsDictionary[SlotId].FindGroupType(drawing.Numbers[SlotId - 1]):
                    SubSets.Zero;
                Template.Add(slotset);
                Value += ((int)slotset * (int)Math.Pow(10, SlotId));
            }
        }
        public int GetValue() => Value;
        public override string ToString()
        {
            return $"{string.Join("-", Template.Select(i => Enum.GetName(typeof(SubSets), i)).ToArray())},{GetValue().ToString()}";
        }
    }
}
