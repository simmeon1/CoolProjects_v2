using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Common_ClassLibrary;

namespace LeagueAPI_ClassLibrary
{
    public interface IMatchSaver
    {
        Task<List<string>> SaveMatches(List<LeagueMatch> matches);
    }

    public class MatchSaver : IMatchSaver
    {
        private readonly IFileIO fileIo;
        private readonly IDDragonRepository repository;
        private readonly IExcelPrinter excelPrinter;
        private readonly ILogger logger;
        private readonly IDateTimeProvider dateTimeProvider;
        private readonly string outputDirectory;
        private readonly int queueId;
        private readonly List<string> targetVersions;
        private List<int> includeWinRatesForMinutes;
        private readonly ILeagueAPIClient leagueApiClient;

        public MatchSaver(
            ILeagueAPIClient leagueApiClient,
            IFileIO fileIo,
            IDDragonRepository repository,
            IExcelPrinter excelPrinter,
            ILogger logger,
            IDateTimeProvider dateTimeProvider,
            string outputDirectory,
            int queueId,
            List<string> targetVersions,
            List<int> includeWinRatesForMinutes) {
            this.fileIo = fileIo;
            this.repository = repository;
            this.excelPrinter = excelPrinter;
            this.logger = logger;
            this.dateTimeProvider = dateTimeProvider;
            this.outputDirectory = outputDirectory;
            this.queueId = queueId;
            this.targetVersions = targetVersions;
            this.includeWinRatesForMinutes = includeWinRatesForMinutes;
            this.leagueApiClient = leagueApiClient;
        }

        public async Task<List<string>> SaveMatches(List<LeagueMatch> matches) {
            string queueName = await leagueApiClient.GetNameOfQueue(queueId);
            string versionsStr = targetVersions.ConcatenateListOfStringsToCommaString();
            string idString = $"{queueName}_{versionsStr}_{Globals.GetDateTimeFileNameFriendly(dateTimeProvider.Now())}";
            string resultsFolder = $"Results_{idString}";
            string path = Path.Combine(outputDirectory, resultsFolder);
            if (!fileIo.DirectoryExists(path)) fileIo.CreateDirectory(path);
            string matchesFilePath = Path.Combine(path, $"Matches_{idString}.json");
            string itemSetFilePath = Path.Combine(path, $"ItemSet_All_{idString}.json");
            string statsFilePath = Path.Combine(path, $"Stats_{idString}.xlsx");
            string logFilePath = Path.Combine(path, $"Log_{idString}.txt");
            
            List<string> createdFiles = new();
            
            fileIo.WriteAllText(matchesFilePath, matches.SerializeObject());
            createdFiles.Add(matchesFilePath);

            includeWinRatesForMinutes ??= new List<int>();
            DataCollector collector = new();

            List<KeyValuePair<int, DataCollectorResults>> resultsList = new()
            {
                new KeyValuePair<int, DataCollectorResults>(0, collector.GetData(matches))
            };
            
            foreach (int minute in includeWinRatesForMinutes)
            {
                resultsList.Add(
                    new KeyValuePair<int, DataCollectorResults>(
                    minute,
                    collector.GetData(matches.Where(m => m.GameIsShorterThanOrEqualToMinutes(minute)).ToList()))
                );
            }

            ItemSetExporter exporter = new(repository);
            for (int i = 0; i < resultsList.Count; i++)
            {
                int resultMinute = resultsList[i].Key;
                DataCollectorResults resultData = resultsList[i].Value;
                Dictionary<int, WinLossData> itemData = resultData.GetItemData();
                string itemSetFileName = resultMinute == 0
                    ? itemSetFilePath
                    : itemSetFilePath.Replace("All", $"Sub{resultMinute}");
                string itemSetJson = exporter.GetItemSet(itemData, Path.GetFileNameWithoutExtension(itemSetFileName));
                fileIo.WriteAllText(itemSetFileName, itemSetJson);
                createdFiles.Add(itemSetFileName);
            }

            DataTableCreator dataTableCreator = new(repository);
            List<DataTable> dataTables = new()
            {
                dataTableCreator.GetChampionTable(GetResultsDict(resultsList, (x) => x.GetChampionData())),
                dataTableCreator.GetItemTable(GetResultsDict(resultsList, (x) => x.GetItemData())),
                dataTableCreator.GetRuneTable(GetResultsDict(resultsList, (x) => x.GetRuneData())),
                dataTableCreator.GetStatPerkTable(GetResultsDict(resultsList, (x) => x.GetStatPerkData())),
                dataTableCreator.GetSpellTable(GetResultsDict(resultsList, (x) => x.GetSpellData()))
            };

            excelPrinter.PrintTablesToWorksheet(dataTables, statsFilePath);
            createdFiles.Add(statsFilePath);
            
            fileIo.WriteAllText(logFilePath, logger.GetContent());
            createdFiles.Add(logFilePath);
            
            logger.Log($"{createdFiles.Count} files written at {path}.");
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