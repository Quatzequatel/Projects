using System;
using System.Collections.Generic;
using System.Linq;

namespace LotteryV2.Domain.Commands
{
    public class CommandFactory : ICommandFactory<DrawingContext>
    {
        public LinkedList<Command<DrawingContext>> CreateAlternateCommands(DrawingContext context)
        {
            return Resolve(AlternateCommands().ToList());
        }

        public LinkedList<Command<DrawingContext>> CreateCommands(DrawingContext context)
        {
            return Resolve(GetCommandTypes(context).ToList());
        }

        private static IEnumerable<Type> GetCommandTypes(DrawingContext context) =>
            context.SkipDownload ? SkipDownloadCommands() : DefaultCommands;

        public LinkedList<Command<DrawingContext>> Resolve(List<Type> types)
        {
            return new LinkedList<Command<DrawingContext>>(types.Select(t => (Command<DrawingContext>)t));
        }

        private static IEnumerable<Type> DefaultCommands => new List<Type>
        {
            typeof(DefineDrawingDateRangeCommand), //define context TBD.
            typeof(LoadDrawingsFromFile), //load drawings from json file; if exists

            //Load new data from Web.
            typeof(ScrapeFromWeb), //if no json file scrape from web and save.
            typeof(UpdateJsonFromWeb), // update json file by appending drawings.
            typeof(SaveJsonToFileCommand), //save to file.
            typeof(SaveToDBCommand), //Save to SQL DB.

            typeof(LoadFilehistoricalPeriods), //??
            typeof(DefineHistoricalGroups),
            typeof(SaveGroupsDictionaryToCSVCommand),
            typeof(SetTopPatternsCommand),
            typeof(SetHistoricalPeriodsCommand), //Save to json.
            typeof(PastDrawingReportCommand), //PastDrawingsReport, Save PastDrawingsReport to csv file.
            typeof(SaveHistoricalPatternSummary), // Save Save Historical Pattern Summary to csv.
            typeof(PickNumbersBaseCommand), //Get Best numbers to pick.
            typeof(SetTemplateFingerPrintCommand),
            typeof(SaveGroups2JsonCommand), //Save groups to json ** possible BS code.
            typeof(TemplateSetsReportCommand),
            typeof(SaveDrawingTemplateToCSVCommand), //Create CSV report of Drawing Templates.
            //typeof(PurmutateNumbers),
            //typeof(NumberGeneratorCommand),
            typeof(SaveBaseCSVCommand), //save to CSV file.
            typeof(SlotNumberAnalysis2CSVCommand),
            typeof(SlotNumberAnalysisRanged2CSVCommand)
        };

        private static IEnumerable<Type> SkipDownloadCommands() => new List<Type>
        {
            typeof(DefineDrawingDateRangeCommand), //define context TBD.
            typeof(LoadDrawingsFromFile), //load drawings from json file; if exists

            ////Load new data from Web.
            //typeof(ScrapeFromWeb), //if no json file scrape from web and save.
            //typeof(UpdateJsonFromWeb), // update json file by appending drawings.
            //typeof(SaveJsonToFileCommand), //save to file.
            //typeof(SaveToDBCommand), //Save to SQL DB.

            typeof(LoadFilehistoricalPeriods), //??
            typeof(DefineHistoricalGroups),
            typeof(SaveGroupsDictionaryToCSVCommand),
            typeof(SetTopPatternsCommand),
            typeof(SetHistoricalPeriodsCommand), //Save to json.
            typeof(PastDrawingReportCommand), //PastDrawingsReport, Save PastDrawingsReport to csv file.
            typeof(SaveHistoricalPatternSummary), // Save Save Historical Pattern Summary to csv.
            typeof(PickNumbersBaseCommand), //Get Best numbers to pick.
            typeof(SetTemplateFingerPrintCommand),
            typeof(SaveGroups2JsonCommand), //Save groups to json ** possible BS code.
            typeof(TemplateSetsReportCommand),
            typeof(SaveDrawingTemplateToCSVCommand), //Create CSV report of Drawing Templates.
            //typeof(PurmutateNumbers),
            //typeof(NumberGeneratorCommand),
            typeof(SaveBaseCSVCommand), //save to CSV file.
            typeof(SlotNumberAnalysis2CSVCommand),
            typeof(SlotNumberAnalysisRanged2CSVCommand)
        };

        private static IEnumerable<Type> AlternateCommands()
        {
             throw new NotImplementedException();
        }
    }
}
