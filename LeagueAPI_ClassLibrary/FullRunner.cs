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

        public List<string> DoFullRun(string outputDirectory, string existingMatchesFilePath, List<int> includeWinRatesForMinutes = null)
        {
            InitialiseFileNames(outputDirectory);
            List<string> createdFiles = new();

            try
            {
                List<LeagueMatch> matches = FileIO.ReadAllText(existingMatchesFilePath).DeserializeObject<List<LeagueMatch>>();
                return GetCreatedFilesAfterMatchAnalysis(createdFiles, matches, includeWinRatesForMinutes);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
                return CreateLogFileAndReturnListOfCreatedFiles(createdFiles);
            }
        }

        public async Task<List<string>> DoFullRun(string outputDirectory, int queueId, string startPuuid, string targetVersion = null, int maxCount = 0, List<int> includeWinRatesForMinutes = null)
        {
            InitialiseFileNames(outputDirectory);
            List<string> createdFiles = new();

            try
            {
                List<LeagueMatch> matches = await MatchCollector.GetMatches(startPuuid, queueId, targetVersion, maxCount);
                FileIO.WriteAllText(MatchesFilePath, matches.SerializeObject());
                createdFiles.Add(MatchesFilePath);
                return GetCreatedFilesAfterMatchAnalysis(createdFiles, matches, includeWinRatesForMinutes);
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
            ItemSetFilePath = Path.Combine(outputDirectory, $"ItemSet_All_{idString}.json");
            StatsFilePath = Path.Combine(outputDirectory, $"Stats_{idString}.xlsx");
            LogFilePath = Path.Combine(outputDirectory, $"Log_{idString}.txt");
        }

        private List<string> GetCreatedFilesAfterMatchAnalysis(List<string> createdFiles, List<LeagueMatch> matches, List<int> includeWinRatesForMinutes)
        {
            if (includeWinRatesForMinutes == null) includeWinRatesForMinutes = new();
            DataCollector collector = new();

            List<KeyValuePair<int,DataCollectorResults>> resultsList = new();
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
