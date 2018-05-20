using System.Collections.Generic;
using System.Linq;

namespace LotteryV2.Domain
{
    public class Templates
    {
        private Dictionary<TemplateSets, List<FingerPrint>> _templates = new Dictionary<TemplateSets, List<FingerPrint>>();

        public void AddDrawings(IEnumerable<Drawing> drawings)
        {
            Dictionary<int, int> fingerprintsByCount = new Dictionary<int, int>();
            foreach (var item in drawings
                .GroupBy(i => i.TemplateFingerPrint.GetValue())
                .Select(group => new { key = group.Key, count = group.Count() })
                .OrderBy(x => x.count))
            {
                //drawings.ToList().ForEach(i => i.GetTemplateFingerPrint().Count = item.count);
            }

            drawings.ToList().Where(i => i.GetTemplateFingerPrint().TimesChoosen <= (int)TemplateSets.Aqua)
                .ToList().ForEach(j => j.GetTemplateFingerPrint().TemplateSet = TemplateSets.Aqua);

            drawings.ToList()
                .Where(i => i.GetTemplateFingerPrint().TimesChoosen > (int)TemplateSets.Aqua
                && i.GetTemplateFingerPrint().TimesChoosen <= (int)TemplateSets.Sunrise)
                .ToList().ForEach(j => j.GetTemplateFingerPrint().TemplateSet = TemplateSets.Sunrise);

            drawings.ToList()
                .Where(i => i.GetTemplateFingerPrint().TimesChoosen > (int)TemplateSets.Sunrise)
                .ToList().ForEach(j => j.GetTemplateFingerPrint().TemplateSet = TemplateSets.RedHot);

        }
    }
}
