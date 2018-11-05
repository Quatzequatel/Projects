using System.Collections.Generic;
using System.Linq;
using System;
using Newtonsoft.Json;
using LotteryV2.Domain.Extensions;

namespace LotteryV2.Domain.Model
{
    public class FingerPrint
    {
        private List<SubSets> Template { get; set; } = new List<SubSets>();

        public List<SubSets> GetTemplate() => Template;

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
                    Value += ((int)Template[slotId - 1] * (int)Math.Pow(10, slotId));
                }
            }
        }

        /// <summary>
        /// number of times template has been chosen.
        /// </summary>
        public int TimesChoosen { get => DrawingDates.Count(); }

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

        //public DateTime DrawingDate { get; set; }
        public List<DateTime> DrawingDates { get; set; } = new List<DateTime>();

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

        /// <summary>
        /// Uses Context.GroupDictionary to look up SubSet for each slot.
        /// also calculates subset collection unique value.
        /// </summary>
        /// <param name="drawing"></param>
        public FingerPrint(Drawing drawing)
        {
            DrawingDates.Add(drawing.DrawingDate);

            for (int SlotId = 1; SlotId <= drawing.Numbers.Length; SlotId++)
            {
                SubSets slotset = drawing.Context.GroupsDictionary[SlotId]?.FindGroupType(drawing.Numbers[SlotId - 1]) != null ?
                    drawing.Context.GroupsDictionary[SlotId].FindGroupType(drawing.Numbers[SlotId - 1]) :
                    SubSets.Zero;
                Template.Add(slotset);
                Value += ((int)slotset * (int)Math.Pow(10, SlotId));
            }
        }

        public string CSVheadings() => $"Template, Key Value, Count";
        public override string ToString()
        {
            return $"{string.Join("-", Template.Select(i => Enum.GetName(typeof(SubSets), i)).ToArray())},{GetValue().ToString()}, {TimesChoosen.ToString()}";
        }

        public FingerPrint Clone()
        {
            FingerPrint clone = new FingerPrint()
            {
                Template = Template.ToList(),
                Value = Value,
                DrawingDates = DrawingDates.ToList(),
                TemplateSet = (TemplateSets)Enum.Parse(typeof(TemplateSets), this.TemplateSet.ToString())
            };

            return clone;
        }
    }

    public class ThingAbob
    {
        FingerPrint FingerPrint { get; set; }

        List<DateTime> DrawingDates { get; set; }

        int Count = 0;

        public void AddDrawingDate(DateTime drawingDate)
        {
            DrawingDates.Add(drawingDate);
            Count++;
        }

        public int KeyValue => FingerPrint.Value;
    }
}
