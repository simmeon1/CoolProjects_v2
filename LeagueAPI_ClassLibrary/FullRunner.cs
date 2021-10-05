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
        private IFileIO FileIO { get; set; }
        private IDateTimeProvider DateTimeProvider { get; set; }
        private IGuidProvider GuidProvider { get; set; }
        private IExcelPrinter ExcelPrinter { get; set; }
        private ILogger Logger { get; set; }
        private string MatchesFilePath { get; set; }
        private string ItemSetFilePath { get; set; }
        private string StatsFilePath { get; set; }
        private string LogFilePath { get; set; }

        public FullRunner(IMatchCollector matchCollector, IDDragonRepository repository, IFileIO fileIO, IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider, IExcelPrinter excelPrinter, ILogger logger)
        {
            MatchCollector = matchCollector;
            Repository = repository;
            FileIO = fileIO;
            DateTimeProvider = dateTimeProvider;
            GuidProvider = guidProvider;
            ExcelPrinter = excelPrinter;
            Logger = logger;
        }

        public List<string> DoFullRun(string outputDirectory, string existingMatchesFilePath)
        {
            InitialiseFileNames(outputDirectory);
            List<string> createdFiles = new();

            try
            {
                List<LeagueMatch> matches = FileIO.ReadAllText(existingMatchesFilePath).DeserializeObject<List<LeagueMatch>>();
                return GetCreatedFilesAfterMatchAnalysis(createdFiles, matches);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return CreateLogFileAndReturnListOfCreatedFiles(createdFiles);
            }
        }

        public async Task<List<string>> DoFullRun(string outputDirectory, int queueId, string startPuuid, string targetVersion = null, int maxCount = 0)
        {
            InitialiseFileNames(outputDirectory);
            List<string> createdFiles = new();

            try
            {
                List<LeagueMatch> matches = await MatchCollector.GetMatches(startPuuid, queueId, targetVersion, maxCount);
                FileIO.WriteAllText(MatchesFilePath, matches.SerializeObject());
                createdFiles.Add(MatchesFilePath);
                return GetCreatedFilesAfterMatchAnalysis(createdFiles, matches);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return CreateLogFileAndReturnListOfCreatedFiles(createdFiles);
            }
        }

        private void InitialiseFileNames(string outputDirectory)
        {
            string idString = Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(DateTimeProvider.Now(), GuidProvider.NewGuid());
            MatchesFilePath = Path.Combine(outputDirectory, $"Matches_{idString}.json");
            ItemSetFilePath = Path.Combine(outputDirectory, $"ItemSet_{idString}.json");
            StatsFilePath = Path.Combine(outputDirectory, $"Stats_{idString}.xlsx");
            LogFilePath = Path.Combine(outputDirectory, $"Log_{idString}.txt");
        }

        private List<string> GetCreatedFilesAfterMatchAnalysis(List<string> createdFiles, List<LeagueMatch> matches)
        {
            DataCollector collector = new();
            DataCollectorResults results = collector.GetData(matches);
            Dictionary<int, WinLossData> itemData = results.GetItemData();
            ItemSetExporter exporter = new(Repository);
            string itemSetJson = exporter.GetItemSet(itemData, Path.GetFileNameWithoutExtension(ItemSetFilePath));
            FileIO.WriteAllText(ItemSetFilePath, itemSetJson);
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
            ExcelPrinter.PrintTablesToWorksheet(dataTables, StatsFilePath);
            createdFiles.Add(StatsFilePath);
            return CreateLogFileAndReturnListOfCreatedFiles(createdFiles);
        }

        private List<string> CreateLogFileAndReturnListOfCreatedFiles(List<string> createdFiles)
        {
            FileIO.WriteAllText(LogFilePath, Logger.GetContent());
            createdFiles.Add(LogFilePath);
            return createdFiles;
        }
    }
}
