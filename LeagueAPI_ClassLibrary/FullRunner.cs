using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class FullRunner
    {
        private IMatchCollector MatchCollector { get; set; }
        private IDDragonRepository Repository { get; set; }
        private string MatchesFilePath { get; set; }
        private string ItemSetFilePath { get; set; }
        private string StatsFilePath { get; set; }

        public FullRunner(IMatchCollector matchCollector, IDDragonRepository repository)
        {
            MatchCollector = matchCollector;
            Repository = repository;
        }

        public List<string> DoFullRun(string outputDirectory, string existingMatchesFilePath)
        {
            InitialiseFileNames(outputDirectory);
            List<string> createdFiles = new();
            List<LeagueMatch> matches = File.ReadAllText(existingMatchesFilePath).DeserializeObject<List<LeagueMatch>>();
            return GetCreatedFilesAfterMatchAnalysis(createdFiles, matches);
        }

        public async Task<List<string>> DoFullRun(string outputDirectory, int queueId, string startPuuid, string targetVersion = null, int maxCount = 0)
        {
            InitialiseFileNames(outputDirectory);
            List<string> createdFiles = new();
            List<LeagueMatch> matches = await MatchCollector.GetMatches(startPuuid, queueId, targetVersion, maxCount);
            File.WriteAllText(MatchesFilePath, matches.SerializeObject());
            createdFiles.Add(MatchesFilePath);
            return GetCreatedFilesAfterMatchAnalysis(createdFiles, matches);
        }

        private void InitialiseFileNames(string outputDirectory)
        {
            DateTime startOfRun = DateTime.Now;
            string startOfRunStr = startOfRun.ToString("yyyy-MM-dd--HH-mm-ss");
            string runGuid = Guid.NewGuid().ToString();
            MatchesFilePath = Path.Combine(outputDirectory, $"Matches_{startOfRunStr}_{runGuid}.json");
            ItemSetFilePath = Path.Combine(outputDirectory, $"ItemSet_{startOfRunStr}_{runGuid}.json");
            StatsFilePath = Path.Combine(outputDirectory, $"Stats_{startOfRunStr}_{runGuid}.xlsx");
        }

        private List<string> GetCreatedFilesAfterMatchAnalysis(List<string> createdFiles, List<LeagueMatch> matches)
        {
            DataCollector collector = new();
            DataCollectorResults results = collector.GetData(matches);
            Dictionary<int, WinLossData> itemData = results.GetItemData();
            ItemSetExporter exporter = new(Repository);
            string itemSetJson = exporter.GetItemSet(itemData);
            File.WriteAllText(ItemSetFilePath, itemSetJson);
            createdFiles.Add(ItemSetFilePath);

            DataTableCreator dataTableCreator = new(Repository);
            List<DataTable> dataTables = new()
            {
                dataTableCreator.GetChampionTable(results.GetChampionData()),
                dataTableCreator.GetItemTable(itemData),
                dataTableCreator.GetRuneTable(results.GetRuneData()),
                dataTableCreator.GetStatPerkTable(results.GetStatPerkData()),
                dataTableCreator.GetSpellTable(results.GetSpellData())
            };

            ExcelPrinter printer = new();
            printer.PrintTablesToWorksheet(dataTables, StatsFilePath);
            createdFiles.Add(StatsFilePath);
            return createdFiles;
        }
    }
}
