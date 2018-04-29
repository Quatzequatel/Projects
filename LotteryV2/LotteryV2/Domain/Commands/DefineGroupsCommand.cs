using System.Linq;
using Newtonsoft.Json;

namespace LotteryV2.Domain.Commands
{
    class DefineGroupsCommand : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            context.DefineGroups();
            context.Drawings.ForEach(i => i.SetContext(context));
        }
    }

    class SetTemplateFingerPrintCommand : Command<DrawingContext>
    {
        public override void Execute(DrawingContext context)
        {
            context.Drawings.ForEach(i => i.GetTemplateFingerPrint());
        }
    }
    class SaveGroups2JsonCommand : Command<DrawingContext>
    {
        private string _Filename;
        public override void Execute(DrawingContext context)
        {
            _Filename = $"{context.FilePath}{context.GetGameName()}-Groups.json";
            SaveToJson(context);
        }

        private void SaveToJson(DrawingContext context)
        {
            System.IO.File.WriteAllText(_Filename, JsonConvert.SerializeObject(context.GroupsDictionary.Select(i=>i.Value).ToArray(), Formatting.Indented));
        }
    }
}
