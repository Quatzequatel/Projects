using LotteryV2.Domain.Commands;
using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;

namespace LotteryV2.Domain
{
    public class FingerPrint
    {
        private List<SubSets> Template { get; set; } = new List<SubSets>();

        /// <summary>
        /// public wrapper for Template property.
        /// </summary>
        public string[] TemplateToString
        {
            get
            {
                return Template.ToArray().Select(i => Enum.GetName(typeof(SubSets), i)).ToArray();
            }
            set
            {
                Template = value.Select(i => (SubSets)Enum.Parse(typeof(SubSets), i)).ToList<SubSets>();
                for (int slotId = 1; slotId <= new int[0].GetSlotCount(); slotId++)
                {
                    Value += ((int)Template[slotId-1] * (int)Math.Pow(10, slotId));
                }
            }
        }

        /// <summary>
        /// number of times template has been chosen.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// unique int the Template result. Generally, the higher the value,
        /// the higher the propability.
        /// </summary>
        /// <returns></returns>
        public int GetValue() => Value;

        /// <summary>
        /// See GetValue()
        /// </summary>
        public int Value { get; set; }

        [JsonIgnore]
        /// <summary>
        /// TBD
        /// </summary>
        public TemplateSets TemplateSet { get; set; }

        /// <summary>
        /// json instanciator. 
        /// </summary>
        public FingerPrint()
        {

        }


        public FingerPrint(Drawing drawing)
        {

            for (int SlotId = 1; SlotId <= drawing.Numbers.Length; SlotId++)
            {
                SubSets slotset = drawing.Context.GroupsDictionary[SlotId]?.FindGroupType(drawing.Numbers[SlotId - 1]) != null ?
                    drawing.Context.GroupsDictionary[SlotId].FindGroupType(drawing.Numbers[SlotId - 1]) :
                    SubSets.Zero;
                Template.Add(slotset);
                Value += ((int)slotset * (int)Math.Pow(10, SlotId));
            }
        }
        public override string ToString()
        {
            return $"{string.Join("-", Template.Select(i => Enum.GetName(typeof(SubSets), i)).ToArray())},{GetValue().ToString()}";
        }
    }

    public class HistoricalPeriodsJson
    {
        public DateTime DrawingDate { get; set; }
        public int[] Numbers { get; set; }
        public string KeyString { get; set; }
        public KeyValuePair<string,FingerPrint>[] JsonHistoricalFingerPrints { get; set; }

        public HistoricalPeriodsJson()
        {

        }
    }
}
