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
        private IDdragonRepositoryUpdater RepoUpdater { get; set; }
        private IFileIO FileIO { get; set; }
        private IDateTimeProvider DateTimeProvider { get; set; }
        private IGuidProvider GuidProvider { get; set; }
        private IExcelPrinter ExcelPrinter { get; set; }
        private ILogger Logger { get; set; }
        private string MatchesFilePath { get; set; }
        private string ItemSetFilePath { get; set; }
        private string StatsFilePath { get; set; }
        private string LogFilePath { get; set; }

        public FullRunner(
            IMatchCollector matchCollector,
            IDDragonRepository repository,
            IFileIO fileIO,
            IDateTimeProvider dateTimeProvider,
            IGuidProvider guidProvider,
            IExcelPrinter excelPrinter,
            ILogger logger,
            IDdragonRepositoryUpdater repoUpdater)
        {
            MatchCollector = matchCollector;
            Repository = repository;
            FileIO = fileIO;
            DateTimeProvider = dateTimeProvider;
            GuidProvider = guidProvider;
            ExcelPrinter = excelPrinter;
            Logger = logger;
            RepoUpdater = repoUpdater;
        }

        public async Task<List<string>> DoFullRun(string outputDirectory, int queueId, string startPuuid, List<string> targetVersions, int maxCount, List<int> includeWinRatesForMinutes, string existingMatchesFile, bool getLatestVersion)
        {
            List<string> createdFiles = new();
            try
            {
                InitialiseFileNames(outputDirectory);
                List<LeagueMatch> alreadyScannedMatches = ReadExistingMatches(existingMatchesFile);

                targetVersions = await RepoUpdater.GetParsedListOfVersions(targetVersions);
                Task updateTask = Task.CompletedTask;
                if (getLatestVersion) updateTask = RepoUpdater.GetLatestDdragonFiles();

                List<LeagueMatch> matches = await MatchCollector.GetMatches(startPuuid, queueId, targetVersions, maxCount, alreadyScannedMatches);
                await updateTask;
                Repository.RefreshData();
                return SaveFiles(createdFiles, matches, includeWinRatesForMinutes);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                return CreateLogFileAndReturnListOfCreatedFiles(createdFiles);
            }
        }

        private List<LeagueMatch> ReadExistingMatches(string existingMatchesFile)
        {
            List<LeagueMatch> alreadyScannedMatches = null;
            bool matchesProvided = !existingMatchesFile.IsNullOrEmpty();
            if (matchesProvided)
            {
                Logger.Log("Reading already scanned matches...");
                alreadyScannedMatches = FileIO.ReadAllText(existingMatchesFile).DeserializeObject<List<LeagueMatch>>();
            }
            return alreadyScannedMatches;
        }

        private void InitialiseFileNames(string outputDirectory)
        {
            string idString = Globals.GetDateTimeFileNameFriendlyConcatenatedWithString(DateTimeProvider.Now(), GuidProvider.NewGuid());
            string resultsFolder = $"Results_{idString}";
            string path = Path.Combine(outputDirectory, resultsFolder);
            if (!FileIO.DirectoryExists(path)) FileIO.CreateDirectory(path);
            MatchesFilePath = Path.Combine(path, $"Matches_{idString}.json");
            ItemSetFilePath = Path.Combine(path, $"ItemSet_All_{idString}.json");
            StatsFilePath = Path.Combine(path, $"Stats_{idString}.xlsx");
            LogFilePath = Path.Combine(path, $"Log_{idString}.txt");
        }

        private List<string> SaveFiles(List<string> createdFiles, List<LeagueMatch> matches, List<int> includeWinRatesForMinutes)
        {
            FileIO.WriteAllText(MatchesFilePath, matches.SerializeObject());
            createdFiles.Add(MatchesFilePath);

            if (includeWinRatesForMinutes == null) includeWinRatesForMinutes = new();
            DataCollector collector = new();

            List<KeyValuePair<int, DataCollectorResults>> resultsList = new();
            resultsList.Add(new KeyValuePair<int, DataCollectorResults>(0, collector.GetData(matches)));
            foreach (int minute in includeWinRatesForMinutes)
            {
                resultsList.Add(new KeyValuePair<int, DataCollectorResults>(minute, collector.GetData(matches.Where(m => m.GameIsShorterThanOrEqualToMinutes(minute)).ToList())));
            }

            ItemSetExporter exporter = new(Repository);
            for (int i = 0; i < resultsList.Count; i++)
            {
                int resultMinute = resultsList[i].Key;
                DataCollectorResults resultData = resultsList[i].Value;
                Dictionary<int, WinLossData> itemData = resultData.GetItemData();
                string itemSetFileName = resultMinute == 0 ? ItemSetFilePath : ItemSetFilePath.Replace("All", $"Sub{resultMinute}");
                string itemSetJson = exporter.GetItemSet(itemData, Path.GetFileNameWithoutExtension(itemSetFileName));
                FileIO.WriteAllText(itemSetFileName, itemSetJson);
                createdFiles.Add(itemSetFileName);
            }

            DataTableCreator dataTableCreator = new(Repository);
            List<DataTable> dataTables = new()
            {
                dataTableCreator.GetChampionTable(GetResultsDict(resultsList, (x) => x.GetChampionData())),
                dataTableCreator.GetItemTable(GetResultsDict(resultsList, (x) => x.GetItemData())),
                dataTableCreator.GetRuneTable(GetResultsDict(resultsList, (x) => x.GetRuneData())),
                dataTableCreator.GetStatPerkTable(GetResultsDict(resultsList, (x) => x.GetStatPerkData())),
                dataTableCreator.GetSpellTable(GetResultsDict(resultsList, (x) => x.GetSpellData()))
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

        private static Dictionary<int, Dictionary<int, WinLossData>> GetResultsDict(List<KeyValuePair<int, DataCollectorResults>> pairs, Func<DataCollectorResults, Dictionary<int, WinLossData>> func)
        {
            Dictionary<int, Dictionary<int, WinLossData>> dict = new();
            foreach (KeyValuePair<int, DataCollectorResults> pair in pairs)
            {
                dict.Add(pair.Key, func.Invoke(pair.Value));
            }
            return dict;
        }
    }
}
