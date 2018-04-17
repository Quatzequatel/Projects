using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LotteryV2.Domain
{
    public class SlotNumberAnalysis : Number
    {
        public SlotNumberAnalysis(int id, int slotid, Game game) : base(id, slotid, game) { }
        public void LoadDrawingsRange(List<Drawing> drawings, DateTime start, DateTime end)
        {
            LoadDrawings(drawings.Where(i => i.DrawingDate >= start && i.DrawingDate <= end).ToList());
        }
        public void LoadLastNumberOfDrawingsAndLeave(List<Drawing> drawings, int PreviousDrawingsCount, int LeaveDrawingCount)
        {
            int TakeCount = (PreviousDrawingsCount + LeaveDrawingCount) > drawings.Count? drawings.Count - LeaveDrawingCount : PreviousDrawingsCount;
            
            LoadDrawings(drawings.OrderByDescending(i => i.DrawingDate).Take(TakeCount + LeaveDrawingCount).Skip(LeaveDrawingCount).ToList());
        }
        public void LoadDrawings(List<Drawing> drawings)
        {
            foreach (var item in drawings.Where(i => i.Game == Game && i.Numbers[SlotId-1] == Id).ToArray())
            {
                base.AddDrawingDate(item.DrawingDate);
            }
            DrawingsCount = drawings.Count;
        }

        public int TimesChosen => DrawingDates.Count;
        public double PercentChosen => ((double)TimesChosen / DrawingsCount) * 100;

        public decimal TrendlineYvalue { get; set; }

        public int DrawingsCount { get; private set; }
        public void SetDrawingsCount(int value) => DrawingsCount = value;

        public string CSVHeading => new string[] {"Game", "Slot", "Number", "Times Chosen", "Chosen %", "TrendlineYvalue" }.CSV();
        public string CSVLine => new string[]
        {
            $"{Game}",
            $"{SlotId}",
            $"{Id}",
            $"{TimesChosen}",
            $"{PercentChosen}",
            $"{TrendlineYvalue}",
        }.CSV();
    }
}
