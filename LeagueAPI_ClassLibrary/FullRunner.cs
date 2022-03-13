using Common_ClassLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace LeagueAPI_ClassLibrary
{
    public class FullRunner
    {
        private ILeagueAPIClient LeagueApiClient { get; set; }
        private IDDragonRepository Repository { get; set; }
        private IDdragonRepositoryUpdater RepoUpdater { get; set; }
        private IExcelPrinter ExcelPrinter { get; }
        private IDateTimeProvider DateTimeProvider { get; }
        private IFileIO FileIo { get; set; }
        private ILogger Logger { get; set; }

        public FullRunner(ILeagueAPIClient leagueApiClient,
            IDDragonRepository repository,
            IFileIO fileIo,
            ILogger logger,
            IDdragonRepositoryUpdater repoUpdater,
            IExcelPrinter excelPrinter,
            IDateTimeProvider dateTimeProvider
        ) {
            Repository = repository;
            FileIo = fileIo;
            Logger = logger;
            RepoUpdater = repoUpdater;
            ExcelPrinter = excelPrinter;
            DateTimeProvider = dateTimeProvider;
            LeagueApiClient = leagueApiClient;
        }

        public async Task<List<string>> DoFullRun(Parameters p) {
            try
            {
                if (p.GetLatestDdragonData) await RepoUpdater.GetLatestDdragonFiles();
                Repository.RefreshData();
                
                List<string> parsedTargetVersions = await LeagueApiClient.GetParsedListOfVersions(p.RangeOfTargetVersions);
                string queueName = await LeagueApiClient.GetNameOfQueue(p.QueueId);
                
                List<LeagueMatch> alreadyScannedMatches = ReadExistingMatches(p.ExistingMatchesFile);

                string outputDirectory = p.OutputDirectory;
                string versionsStr = parsedTargetVersions.ConcatenateListOfStringsToCommaString();

                MatchSaver matchSaver = new(
                    FileIo, 
                    Repository, 
                    ExcelPrinter, 
                    Logger,
                    DateTimeProvider,
                    outputDirectory,
                    queueName,
                    versionsStr, 
                    p.IncludeWinRatesForMinutes
                );

                MatchAddedHandler matchAddedHandler = new(FileIo, matchSaver, outputDirectory);
                MatchCollector collector = new(LeagueApiClient, Logger, matchAddedHandler);

                List<LeagueMatch> matches = 
                    await collector.GetMatches(p.AccountPuuid, p.QueueId, parsedTargetVersions, p.MaxCount, alreadyScannedMatches);
                return matchSaver.SaveMatches(matches);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.ToString());
                string fileName = Path.Combine(p.OutputDirectory, "leagueApi_log.txt");
                FileIo.WriteAllText(fileName, Logger.GetContent());
                Logger.Log($"Log file written at {p.OutputDirectory}.");
                return new List<string> {fileName};
            }
        }

        private List<LeagueMatch> ReadExistingMatches(string existingMatchesFile)
        {
            List<LeagueMatch> alreadyScannedMatches = null;
            bool matchesProvided = !existingMatchesFile.IsNullOrEmpty();
            if (matchesProvided)
            {
                Logger.Log("Reading already scanned matches...");
                alreadyScannedMatches = FileIo.ReadAllText(existingMatchesFile).DeserializeObject<List<LeagueMatch>>();
            }
            return alreadyScannedMatches;
        }
    }
}